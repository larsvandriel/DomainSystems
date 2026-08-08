namespace Common.Messaging.Abstractions.Requests
{
    public interface ISyncRequestDispatcher
    {
        TResult Dispatch<TResult>(IRequest<TResult> request);
        TResult Dispatch<TRequest, TResult>(TRequest request) where TRequest : IRequest<TResult>;
    }
}
