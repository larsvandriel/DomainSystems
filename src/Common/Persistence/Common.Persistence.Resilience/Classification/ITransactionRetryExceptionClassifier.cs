namespace Common.Persistence.Resilience.Classification
{
    public interface ITransactionRetryExceptionClassifier
    {
        bool ShouldRetry(Exception exception);
    }
}
