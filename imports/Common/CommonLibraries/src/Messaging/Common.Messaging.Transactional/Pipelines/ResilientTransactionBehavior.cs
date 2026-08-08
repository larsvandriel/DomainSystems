using Common.Messaging.Abstractions.Pipelines;
using Common.Messaging.Abstractions.Requests;
using Common.Messaging.Abstractions.Requests.Commands;
using Common.Persistence.Resilience.Execution;
using Common.Results;

namespace Common.Messaging.Transactional.Pipelines;

public sealed class ResilientTransactionBehavior<TRequest>(
    IResilientTransactionExecutor transactionExecutor)
    : IRequestPipelineBehavior<TRequest, Result>
    where TRequest : ITransactionalCommand<Result>
{
    public Task<Result> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<Result> continuation,
        CancellationToken cancellationToken = default)
    {
        return transactionExecutor.ExecuteAsync<IRequestHandler<TRequest, Result>>(
            (handler, attemptCancellationToken) =>
                handler.HandleAsync(request, attemptCancellationToken),
            cancellationToken);
    }
}

public sealed class ResilientTransactionBehavior<TRequest, TValue>(
    IResilientTransactionExecutor transactionExecutor)
    : IRequestPipelineBehavior<TRequest, Result<TValue>>
    where TRequest : ITransactionalCommand<Result<TValue>>
{
    public Task<Result<TValue>> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<Result<TValue>> continuation,
        CancellationToken cancellationToken = default)
    {
        return transactionExecutor.ExecuteAsync<IRequestHandler<TRequest, Result<TValue>>, TValue>(
            (handler, attemptCancellationToken) =>
                handler.HandleAsync(request, attemptCancellationToken),
            cancellationToken);
    }
}
