namespace Common.Persistence.Concurrency
{
    public sealed class ConcurrencyConflictException(string message, Exception? innerException = null) : Exception(message, innerException);
}
