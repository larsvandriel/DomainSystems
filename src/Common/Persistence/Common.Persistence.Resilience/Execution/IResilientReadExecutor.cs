using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Persistence.Resilience.Execution
{
    public interface IResilientReadExecutor
    {
        Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default);
    }
}
