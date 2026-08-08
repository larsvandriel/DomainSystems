using Common.Messaging.Abstractions.Pipelines;
using Common.Messaging.Abstractions.Requests;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Common.Messaging.Sync.Requests
{
    public sealed class SyncRequestDispatcher(IServiceProvider serviceProvider) : ISyncRequestDispatcher
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public TResult Dispatch<TResult>(IRequest<TResult> request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var requestType = request.GetType();

            var method = typeof(SyncRequestDispatcher)
                .GetMethod(nameof(SendTyped), BindingFlags.Instance | BindingFlags.NonPublic)!
                .MakeGenericMethod(requestType, typeof(TResult));

            return (TResult)method.Invoke(this, [request])!;
        }

        public TResult Dispatch<TRequest, TResult>(TRequest request) where TRequest : IRequest<TResult>
        {
            return SendTyped<TRequest, TResult>(request);
        }

        private TResult SendTyped<TRequest, TResult>(TRequest request) where TRequest : IRequest<TResult>
        {
            ArgumentNullException.ThrowIfNull(request);

            var handler = _serviceProvider.GetRequiredService<ISyncRequestHandler<TRequest, TResult>>();

            var behaviours = _serviceProvider.GetServices<ISyncRequestPipelineBehavior<TRequest, TResult>>().Reverse().ToArray();

            SyncRequestHandlerDelegate<TResult> pipeline = () => handler.Handle(request);

            foreach (var behaviour in behaviours)
            {
                var next = pipeline;
                pipeline = () => behaviour.Handle(request, next);
            }

            return pipeline();
        }
    }
}
