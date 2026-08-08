using Common.Persistence.Concurrency;

namespace Common.Persistence.Resilience.Classification
{
    public sealed class ConcurrencyConflictExceptionClassifier : ITransactionRetryExceptionClassifier
    {
        public bool ShouldRetry(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            return exception is ConcurrencyConflictException;
        }
    }
}
