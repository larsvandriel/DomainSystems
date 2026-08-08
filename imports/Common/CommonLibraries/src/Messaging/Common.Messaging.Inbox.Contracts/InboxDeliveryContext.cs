namespace Common.Messaging.Inbox.Contracts
{
    public sealed record InboxDeliveryContext
    {
        public int DeliveryAttempt { get; }
        public int? MaximumDeliveryAttempts { get; }
        public string? TraceId { get; init; }

        public bool IsFinalAttempt => MaximumDeliveryAttempts.HasValue && DeliveryAttempt >= MaximumDeliveryAttempts.Value;

        public InboxDeliveryContext(int deliveryAttempt, int? maximumDeliveryAttempts = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(deliveryAttempt);

            if (maximumDeliveryAttempts.HasValue)
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumDeliveryAttempts.Value);

            DeliveryAttempt = deliveryAttempt;
            MaximumDeliveryAttempts = maximumDeliveryAttempts;
        }
    }
}
