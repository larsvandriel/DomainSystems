using Common.Messaging.Abstractions.Requests;

namespace Common.Messaging.Abstractions.Pipelines
{
    public interface ISyncRequestPipelineBehavior<TRequest, TResult> where TRequest : IRequest<TResult>
    {
        TResult Handle(TRequest request, SyncRequestHandlerDelegate<TResult> continuation);
    }
}
