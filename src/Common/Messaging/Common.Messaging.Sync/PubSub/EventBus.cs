using Common.Messaging.Abstractions.PubSub;
using System.Collections.Concurrent;

namespace Common.Messaging.Sync.PubSub
{
    public sealed class EventBus : IEventBus
    {
        private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

        public IDisposable Subscribe<TEvent>(Action<TEvent> handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            var eventType = typeof(TEvent);
            var handlers = _handlers.GetOrAdd(eventType, _ => []);

            lock (handlers)
            {
                handlers.Add(handler);
            }

            return new Subscription(() => Unsubscribe(eventType, handler));
        }

        public void Publish<TEvent>(TEvent @event)
        {
            ArgumentNullException.ThrowIfNull(@event);

            if (!_handlers.TryGetValue(typeof(TEvent), out var handlers))
                return;

            Delegate[] snapshot;

            lock (handlers)
            {
                snapshot = [.. handlers];
            }

            foreach (var handler in snapshot.Cast<Action<TEvent>>())
            {
                handler(@event);
            }
        }

        private void Unsubscribe<TEvent>(Type eventType, Action<TEvent> handler)
        {
            if (!_handlers.TryGetValue(eventType, out var handlers))
                return;

            lock (handlers)
            {
                handlers.Remove(handler);

                if (handlers.Count == 0)
                {
                    _handlers.TryRemove(eventType, out _);
                }
            }
        }
        private sealed class Subscription(Action unsubscribe) : IDisposable
        {
            private readonly Action _unsubscribe = unsubscribe;
            private bool _disposed;

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                _unsubscribe();
            }
        }
    }
}
