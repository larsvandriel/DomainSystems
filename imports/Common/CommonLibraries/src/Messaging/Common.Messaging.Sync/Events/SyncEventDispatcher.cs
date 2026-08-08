using Common.Messaging.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.Sync.Events
{
    public sealed class SyncEventDispatcher(IServiceProvider serviceProvider) : ISyncEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
        {
            ArgumentNullException.ThrowIfNull(@event);

            var handlers = _serviceProvider.GetServices<ISyncEventHandler<TEvent>>();

            foreach (var handler in handlers)
            {
                handler.Handle(@event);
            }
        }
    }
}
