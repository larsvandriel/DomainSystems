using Common.Messaging.Abstractions.Pipelines;
using Common.Messaging.Abstractions.Requests;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Common.Messaging.Async.Requests
{
    public sealed class RequestDispatcher(IServiceProvider serviceProvider) : IRequestDispatcher
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public Task<TResult> DispatchAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var requestType = request.GetType();

            var method = typeof(RequestDispatcher)
                .GetMethod(nameof(DispatchTypedAsync), BindingFlags.Instance | BindingFlags.NonPublic)!
                .MakeGenericMethod(requestType, typeof(TResult));

            return (Task<TResult>)method.Invoke(this, [request, cancellationToken])!;
        }

        public Task<TResult> DispatchAsync<TRequest, TResult>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest<TResult>
        {
            return DispatchTypedAsync<TRequest, TResult>(request, cancellationToken);
        }

        private async Task<TResult> DispatchTypedAsync<TRequest, TResult>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest<TResult>
        {
            ArgumentNullException.ThrowIfNull(request);

            dynamic handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResult>>();

            var behaviours = _serviceProvider.GetServices<IRequestPipelineBehavior<TRequest, TResult>>().Reverse().ToArray();

            RequestHandlerDelegate<TResult> pipeline = ct => handler.HandleAsync((dynamic)request, ct);

            foreach (var behaviour in behaviours)
            {
                var next = pipeline;
                pipeline = ct => behaviour.HandleAsync(request, next, ct);
            }

            return await pipeline(cancellationToken);
        }
    }
}
