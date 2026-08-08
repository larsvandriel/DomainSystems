using Common.Messaging.Abstractions.PubSub;
using System.Collections.Concurrent;

namespace Common.Messaging.Async.PubSub
{
    public sealed class AsyncEventBus : IAsyncEventBus
    {
        private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

        public IAsyncDisposable Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            var eventType = typeof(TEvent);
            var handlers = _handlers.GetOrAdd(eventType, _ => []);

            lock (handlers)
            {
                handlers.Add(handler);
            }

            return new AsyncSubscription(() => Unsubscribe(eventType, handler));
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(@event);

            if (!_handlers.TryGetValue(typeof(TEvent), out var handlers))
                return;

            Delegate[] snapshot;

            lock (handlers)
            {
                snapshot = [.. handlers];
            }

            foreach (var handler in snapshot.Cast<Func<TEvent, CancellationToken, Task>>())
            {
                cancellationToken.ThrowIfCancellationRequested();
                await handler(@event, cancellationToken);
            }
        }

        private void Unsubscribe<TEvent>(Type eventType, Func<TEvent, CancellationToken, Task> handler)
        {
            if (!_handlers.TryGetValue(eventType, out var handlers))
            {
                return;
            }

            lock (handlers)
            {
                handlers.Remove(handler);

                if(handlers.Count == 0)
                {
                    _handlers.TryRemove(eventType, out _);
                }
            }
        }

        private sealed class AsyncSubscription(Action unsubscribe) : IAsyncDisposable
        {
            private readonly Action _unsubscribe = unsubscribe;
            private bool _disposed;

            public ValueTask DisposeAsync()
            {
                if (_disposed)
                    return ValueTask.CompletedTask;

                _disposed = true;
                _unsubscribe();
                
                return ValueTask.CompletedTask;
            }
        }
    }
}
