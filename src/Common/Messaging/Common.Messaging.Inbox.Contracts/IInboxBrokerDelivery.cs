namespace Common.Messaging.Inbox.Contracts
{
    public interface IInboxBrokerDelivery
    {
        InboxDeliveryContext Context { get; }

        Task CompleteAsync(CancellationToken cancellationToken = default);

        Task RetryAsync(Exception exception, CancellationToken cancellationToken = default);

        Task DeadLetterAsync(string reason, string description, CancellationToken cancellationToken = default);
    }
}
