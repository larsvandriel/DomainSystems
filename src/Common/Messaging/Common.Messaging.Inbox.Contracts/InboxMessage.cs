namespace Common.Messaging.Inbox.Contracts
{
    public sealed class InboxMessage
    {
        public Guid MessageId { get; private set; }
        public string Consumer { get; private set; } = null!;
        public string MessageType { get; private set; } = null!;
        public DateTimeOffset ReceivedAtUtc { get; private set; }
        public DateTimeOffset ProcessedAtUtc { get; private set; }

        private InboxMessage()
        {
        }

        private InboxMessage(Guid messageId, string consumer, string messageType, DateTimeOffset receivedAtUtc, DateTimeOffset processedAtUtc)
        {
            MessageId = messageId;
            Consumer = consumer;
            MessageType = messageType;
            ReceivedAtUtc = receivedAtUtc;
            ProcessedAtUtc = processedAtUtc;
        }

        public static InboxMessage CreateProcessed(
            Guid messageId,
            string consumer,
            string messageType,
            DateTimeOffset receivedAtUtc,
            DateTimeOffset processedAtUtc)
        {
            if(messageId == Guid.Empty)
            {
                throw new ArgumentException("A message id is required.", nameof(messageId));
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(consumer);
            ArgumentException.ThrowIfNullOrWhiteSpace(messageType);

            if(processedAtUtc< receivedAtUtc)
            {
                throw new ArgumentOutOfRangeException(nameof(processedAtUtc), processedAtUtc, "The Processed time cannot precede the received time.");
            }

            return new InboxMessage(messageId, consumer, messageType, receivedAtUtc, processedAtUtc);
        }
    }
}
