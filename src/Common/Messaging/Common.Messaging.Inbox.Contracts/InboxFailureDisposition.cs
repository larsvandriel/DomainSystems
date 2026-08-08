namespace Common.Messaging.Inbox.Contracts
{
    public enum InboxFailureDisposition
    {
        RetryRequested = 0,
        DeadLetterRequested = 1
    }
}
