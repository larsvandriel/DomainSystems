using Common.Messaging.Integration.Contracts;

namespace Common.Messaging.Outbox.Contracts
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; private set; }
        public DateTimeOffset OccurredAtUtc { get; private set; }
        public string Type { get; private set; } = null!;
        public string Payload { get; private set; } = null!;
        public DateTimeOffset? PublishedAtUtc { get; private set; }
        public int AttemptCount { get; private set; }
        public DateTimeOffset? LastAttemptAtUtc { get; private set; }
        public string? LastError { get; private set; }
        public DateTimeOffset? NextAttemptAtUtc { get; private set; }
        public DateTimeOffset? DeadLetteredAtUtc { get; private set; }

        public int Version { get; private set; }
        public string? CorrelationId { get; private set; }
        public string? CausationId { get; private set; }
        public string? TraceId { get; private set; }

        public bool IsPublished => PublishedAtUtc.HasValue;
        public bool IsDeadLettered => DeadLetteredAtUtc.HasValue;

        private OutboxMessage(
            Guid id,
            DateTimeOffset occurredAtUtc,
            string type,
            int version,
            string payload,
            string? correlationId,
            string? causationId,
            string? traceId,
            DateTimeOffset? publishedAtUtc = null,
            int attemptCount = 0,
            DateTimeOffset? lastAttemptAtUtc = null,
            string? lastError = null,
            DateTimeOffset? nextAttemptAtUtc = null,
            DateTimeOffset? deadLetteredAtUtc = null)
        {
            Id = id;
            OccurredAtUtc = occurredAtUtc;
            Type = type;
            Version = version;
            Payload = payload;
            CorrelationId = correlationId;
            CausationId = causationId;
            TraceId = traceId;
            PublishedAtUtc = publishedAtUtc;
            AttemptCount = attemptCount;
            LastAttemptAtUtc = lastAttemptAtUtc;
            LastError = lastError;
            NextAttemptAtUtc = nextAttemptAtUtc;
            DeadLetteredAtUtc = deadLetteredAtUtc;
        }

        public static OutboxMessage Create(
            Guid id,
            DateTimeOffset occurredAtUtc,
            string type,
            int version,
            string payload,
            string? correlationId = null,
            string? causationId = null,
            string? traceId = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(type);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(version);
            ArgumentNullException.ThrowIfNull(payload);

            return new OutboxMessage(
                id,
                occurredAtUtc,
                type,
                version,
                payload,
                correlationId,
                causationId,
                traceId);
        }

        public static OutboxMessage Restore(
            Guid id,
            DateTimeOffset occurredAtUtc,
            string type,
            int version,
            string payload,
            string? correlationId,
            string? causationId,
            string? traceId,
            DateTimeOffset? publishedAtUtc,
            int attemptCount,
            DateTimeOffset? lastAttemptAtUtc,
            string? lastError,
            DateTimeOffset? nextAttemptAtUtc,
            DateTimeOffset? deadLetteredAtUtc)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(type);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(version);
            ArgumentNullException.ThrowIfNull(payload);
            ArgumentOutOfRangeException.ThrowIfNegative(attemptCount);

            if (deadLetteredAtUtc.HasValue && nextAttemptAtUtc.HasValue)
                throw new ArgumentException("A dead-lettered message cannot have a next attempt.");

            return new OutboxMessage(
                id,
                occurredAtUtc,
                type,
                version,
                payload,
                correlationId,
                causationId,
                traceId,
                publishedAtUtc,
                attemptCount,
                lastAttemptAtUtc,
                lastError,
                nextAttemptAtUtc,
                deadLetteredAtUtc);
        }

        public void MarkPublished(DateTimeOffset publishedAtUtc)
        {
            PublishedAtUtc = publishedAtUtc;
            LastAttemptAtUtc = publishedAtUtc;
            LastError = null;
            AttemptCount++;
            NextAttemptAtUtc = null;
            DeadLetteredAtUtc = null;
        }

        public void MarkFailed(DateTimeOffset attemptedAtUtc, string error, DateTimeOffset? nextAttemptAtUtc, bool deadLetter)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(error);

            LastAttemptAtUtc = attemptedAtUtc;
            LastError = error;
            AttemptCount++;
            NextAttemptAtUtc = nextAttemptAtUtc;

            if (deadLetter)
            {
                DeadLetteredAtUtc = attemptedAtUtc;
                NextAttemptAtUtc = null;
            }
        }

        public IntegrationEventEnvelope ToEnvelope() => new()
        {
            MessageId = Id,
            Type = Type,
            Version = Version,
            OccurredAtUtc = OccurredAtUtc,
            Payload = Payload,
            CorrelationId = CorrelationId,
            CausationId = CausationId,
            TraceId = TraceId
        };
    }
}
