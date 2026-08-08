namespace Common.Messaging.Inbox.Contracts
{
    public sealed class InboxFailure
    {
        public Guid Id { get; private set; }
        public Guid MessageId { get; private set; }
        public string Consumer { get; private set; } = null!;
        public string MessageType { get; private set; } = null!;
        public int DeliveryAttempt { get; private set; }
        public DateTimeOffset FailedAtUtc { get; private set; }
        public InboxFailureDisposition Disposition { get; private set; }
        public string ErrorType { get; private set; } = null!;
        public string ErrorMessage { get; private set; } = null!;
        public string? ErrorCode { get; private set; }
        public string? TraceId { get; private set; }

        private InboxFailure()
        {
        }

        public static InboxFailure Create(
            Guid messageId,
            string consumer,
            string messageType,
            int deliveryAttempt,
            DateTimeOffset failedAtUtc,
            InboxFailureDisposition disposition,
            string errorType,
            string errorMessage,
            string? errorCode = null,
            string? traceId = null)
        {
            if(messageId == Guid.Empty)
                throw new ArgumentException("A message id is required.", nameof(messageId));

            ArgumentException.ThrowIfNullOrWhiteSpace(consumer);
            ArgumentException.ThrowIfNullOrWhiteSpace(messageType);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(deliveryAttempt);
            ArgumentException.ThrowIfNullOrWhiteSpace(errorType);
            ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

            return new InboxFailure
            {
                Id = Guid.NewGuid(),
                MessageId = messageId,
                Consumer = consumer,
                MessageType = messageType,
                DeliveryAttempt = deliveryAttempt,
                FailedAtUtc = failedAtUtc,
                Disposition = disposition,
                ErrorType = errorType,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                TraceId = traceId
            };
        }
    }
}
