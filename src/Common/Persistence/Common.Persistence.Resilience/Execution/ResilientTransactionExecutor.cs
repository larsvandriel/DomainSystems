using Common.Results;
using Microsoft.Extensions.DependencyInjection;
using Common.Persistence.Transactions.Execution;
using Common.Resilience.Execution;
using Common.Persistence.Resilience.Classification;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Common.Persistence.Transactions.Exceptions;
using Common.Persistence.Resilience.Configuration;

namespace Common.Persistence.Resilience.Execution
{
    public sealed partial class ResilientTransactionExecutor(
        IServiceScopeFactory scopeFactory,
        IRetryExecutor retryExecutor,
        IEnumerable<ITransactionRetryExceptionClassifier> classifiers,
        IOptions<TransactionRetryOptions> options,
        ILogger<ResilientTransactionExecutor> logger) : IResilientTransactionExecutor
    {
        private readonly ITransactionRetryExceptionClassifier[] _classifiers = [.. classifiers];

        public Task<Result> ExecuteAsync<TService>(
            Func<TService, CancellationToken, Task<Result>> action,
            CancellationToken cancellationToken = default)
            where TService : notnull
        {
            ArgumentNullException.ThrowIfNull(action);
            return retryExecutor.ExecuteAsync(action: async ct =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();

                var service = scope.ServiceProvider.GetRequiredService<TService>();

                var transactionExecutor = scope.ServiceProvider.GetRequiredService<ITransactionExecutor>();

                return await transactionExecutor.ExecuteAsync(attemptCt => action(service, attemptCt), ct);
            },
            shouldRetry: ShouldRetry,
            options: options.Value.ToRetryOptions(),
            onRetry: LogRetryAsync,
            cancellationToken);
        }

        public Task<Result<T>> ExecuteAsync<TService, T>(
            Func<TService, CancellationToken, Task<Result<T>>> action,
            CancellationToken cancellationToken = default)
            where TService : notnull
        {
            ArgumentNullException.ThrowIfNull(action);

            return retryExecutor.ExecuteAsync(action: async ct =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();

                var service = scope.ServiceProvider.GetRequiredService<TService>();

                var transactionExecutor = scope.ServiceProvider.GetRequiredService<ITransactionExecutor>();

                return await transactionExecutor.ExecuteAsync(attemptCt => action(service, attemptCt), ct);
            },
            shouldRetry: ShouldRetry,
            options: options.Value.ToRetryOptions(),
            onRetry: LogRetryAsync,
            cancellationToken);
        }

        private bool ShouldRetry(Exception exception)
        {
            if (exception is PostCommitException)
                return false;

            return _classifiers.Any(classifier => classifier.ShouldRetry(exception));
        }

        private Task LogRetryAsync(RetryAttempt attempt, CancellationToken cancellationToken)
        {
            LogRetry(logger, attempt.Exception, attempt.FailedAttempt, attempt.MaximumAttempts, attempt.Delay);

            return Task.CompletedTask;
        }

        [LoggerMessage(
            EventId = 1009,
            Level = LogLevel.Warning,
            Message = "Transaction attempt {Attempt} of {MaximumAttempts} failed. Retrying after {Delay}")]
        private static partial void LogRetry(ILogger logger, Exception exception, int attempt, int maximumAttempts, TimeSpan delay);
    }
}
