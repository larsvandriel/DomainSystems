using Common.Messaging.Abstractions.Events;
using Common.Persistence.Transactions.Abstractions;

namespace Common.Messaging.Transactional
{
    public sealed class PublishEventsAfterCommitParticipant(ITransactionalEventBuffer buffer, IEventDispatcher dispatcher) : ITransactionParticipant
    {
        public Task AbortedAsync(CancellationToken cancellationToken = default)
        {
            buffer.Clear();
            return Task.CompletedTask;
        }

        public async Task CommittedAsync(CancellationToken cancellationToken = default)
        {
            foreach (var @event in buffer.TakeAll())
            {
                await dispatcher.PublishAsync(@event, cancellationToken);
            }
        }

        public Task PrepareAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
