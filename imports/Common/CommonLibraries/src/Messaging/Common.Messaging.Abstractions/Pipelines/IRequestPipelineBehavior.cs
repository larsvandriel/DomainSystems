using Common.Messaging.Abstractions.Requests;

namespace Common.Messaging.Abstractions.Pipelines
{
    public interface IRequestPipelineBehavior<TRequest, TResult> where TRequest : IRequest<TResult>
    {
        Task<TResult> HandleAsync(TRequest request, RequestHandlerDelegate<TResult> continuation, CancellationToken cancellationToken = default);
    }
}
