using Common.Results;

namespace Common.Persistence.Transactions.Execution
{
    public interface ITransactionExecutor
    {
        Task<Result> ExecuteAsync(Func<CancellationToken, Task<Result>> action, CancellationToken cancellationToken = default);

        Task<Result<T>> ExecuteAsync<T>(Func<CancellationToken, Task<Result<T>>> action, CancellationToken cancellationToken = default);
    }
}
