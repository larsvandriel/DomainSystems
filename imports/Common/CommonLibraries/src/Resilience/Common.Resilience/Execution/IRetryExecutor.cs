namespace Common.Resilience.Execution
{
    public interface IRetryExecutor
    {
        Task<T> ExecuteAsync<T>(
            Func<CancellationToken, Task<T>> action,
            Func<Exception, bool> shouldRetry,
            RetryOptions options,
            Func<RetryAttempt, CancellationToken, Task>? onRetry = null,
            CancellationToken cancellationToken = default);
    }
}
