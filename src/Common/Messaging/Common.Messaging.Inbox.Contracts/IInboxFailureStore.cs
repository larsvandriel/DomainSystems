namespace Common.Messaging.Inbox.Contracts
{
    public interface IInboxFailureStore
    {
        void Add(InboxFailure failure);
    }
}
