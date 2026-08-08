namespace Common.Messaging.Abstractions.Requests
{
    public interface ISyncRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
    {
        TResult Handle (TRequest request);
    }
}
