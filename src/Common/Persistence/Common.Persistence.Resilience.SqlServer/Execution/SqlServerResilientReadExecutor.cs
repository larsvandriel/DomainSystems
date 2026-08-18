using System;
using System.Collections.Generic;
using System.Text;
using Common.Persistence.Resilience.Execution;
using Common.Resilience;
using Common.Resilience.Execution;
using Common.Resilience.SqlServer;

namespace Common.Persistence.Resilience.SqlServer.Execution
{
    public sealed class SqlServerResilientReadExecutor(IRetryExecutor retryExecutor) : IResilientReadExecutor
    {
        public Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
        {
            return retryExecutor.ExecuteAsync(
                action,
                SqlServerTransientExceptionDetector.IsTransient,
                RetryOptions.Default,
                cancellationToken: cancellationToken);
        }
    }
}
