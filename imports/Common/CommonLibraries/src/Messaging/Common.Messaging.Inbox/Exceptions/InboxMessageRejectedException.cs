namespace Common.Messaging.Inbox.Exceptions
{
    public sealed class InboxMessageRejectedException(string errorCode, string message) : Exception(message)
    {
        public string ErrorCode { get; } = errorCode;
    }
}
