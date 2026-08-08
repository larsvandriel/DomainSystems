using Common.Persistence.Resilience.Classification;
using Common.Resilience.SqlServer;

namespace Common.Persistence.Resilience.SqlServer.Classification
{
    public sealed class SqlServerTransactionRetryExceptionClassifier : ITransactionRetryExceptionClassifier
    {
        public bool ShouldRetry(Exception exception)
        {
            return SqlServerTransientExceptionDetector.IsTransient(exception);
        }
    }
}
