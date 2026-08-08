namespace Common.Persistence.Transactions.Exceptions
{
    public sealed class PostCommitException(string message, Exception innerException) : Exception(message, innerException);
}
