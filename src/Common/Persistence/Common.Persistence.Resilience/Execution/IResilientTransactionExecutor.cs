using Common.Results;

namespace Common.Persistence.Resilience.Execution
{
    public interface IResilientTransactionExecutor
    {
        Task<Result> ExecuteAsync<TService>(
            Func<TService, CancellationToken, Task<Result>> action,
            CancellationToken cancellationToken = default)
            where TService : notnull;

        Task<Result<T>> ExecuteAsync<TService, T>(
            Func<TService, CancellationToken, Task<Result<T>>> action,
            CancellationToken cancellationToken = default)
            where TService : notnull;
    }
}
