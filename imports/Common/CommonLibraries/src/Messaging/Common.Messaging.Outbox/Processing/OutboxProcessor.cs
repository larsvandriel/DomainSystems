using Common.Messaging.Outbox.Configuration;
using Common.Messaging.Outbox.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Messaging.Outbox.Processing
{
    public sealed partial class OutboxProcessor(
        IOutboxStore store,
        IOutboxTransport transport,
        TimeProvider timeProvider,
        IOptions<OutboxOptions> options,
        ILogger<OutboxProcessor> logger) : IOutboxProcessor
    {
        private readonly OutboxOptions _options = options.Value;

        public async Task<int> ProcessAsync(CancellationToken cancellationToken = default)
        {
            var messages = await store.GetPendingAsync(_options.BatchSize, timeProvider.GetUtcNow(), cancellationToken);

            var publishedCount = 0;

            foreach (var message in messages)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (await ProcessMessageAsync(message, cancellationToken))
                {
                    publishedCount++;
                }
            }

            return publishedCount;
        }

        private async Task<bool> ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken)
        {
            try
            {
                await transport.PublishAsync(message.ToEnvelope(), cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception publishException)
            {
                await RecordPublishFailureAsync(message, publishException);

                return false;
            }

            message.MarkPublished(timeProvider.GetUtcNow());

            try
            {
                await store.SaveChangesAsync(CancellationToken.None);
                return true;
            }
            catch (Exception persistenceException)
            {
                LogPublishedStatusPersistenceFailure(logger, persistenceException, message.Id);
                throw;
            }
        }

        private async Task RecordPublishFailureAsync(OutboxMessage message, Exception publishException)
        {
            var attemptedAtUtc = timeProvider.GetUtcNow();
            var nextAttemptNumber = message.AttemptCount + 1;
            var deadLetter = nextAttemptNumber >= _options.MaximumAttempts;

            DateTimeOffset? nextAttemptAtUtc = deadLetter ? null : attemptedAtUtc + CalculateRetryDelay(nextAttemptNumber);
            
            message.MarkFailed(attemptedAtUtc, publishException.Message, nextAttemptAtUtc, deadLetter);

            try
            {
                await store.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception persistenceException)
            {
                LogFailedAttemptPersistenceFailure(logger, persistenceException, message.Id);
                throw;
            }

            LogPublishFailure(logger, publishException, message.Id);
        }

        private TimeSpan CalculateRetryDelay(int attemptNumber)
        {
            var multiplier = Math.Pow(2, attemptNumber - 1);

            var delayTicks = Math.Min(_options.InitialRetryDelay.Ticks * multiplier, _options.MaximumRetryDelay.Ticks);

            return TimeSpan.FromTicks((long)delayTicks);
        }

        [LoggerMessage(
            EventId = 1001,
            Level = LogLevel.Critical,
            Message = "Outbox message {OutboxMessageId} was published, but its published status could not be persisted. The message may be published again.")]
        private static partial void LogPublishedStatusPersistenceFailure(ILogger logger, Exception exception, Guid outboxMessageId);

        [LoggerMessage(
            EventId = 1002,
            Level = LogLevel.Error,
            Message = "Persisting the failed outbox attempt for message {OutboxMessageId} failed.")]
        private static partial void LogFailedAttemptPersistenceFailure(ILogger logger, Exception exception, Guid outboxMessageId);

        [LoggerMessage(
            EventId = 1003,
            Level = LogLevel.Error,
            Message = "Publishing outbox message {OutboxMessageId} failed.")]
        private static partial void LogPublishFailure(ILogger logger, Exception exception, Guid outboxMessageId);
    }
}
