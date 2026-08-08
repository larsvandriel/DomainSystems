namespace Common.Messaging.Inbox.Contracts
{
    public interface IInboxStore
    {
        Task<InboxMessage?> FindAsync(Guid messageId, string consumer, CancellationToken cancellationToken);

        void Add(InboxMessage message);
    }
}
