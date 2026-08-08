using Common.Messaging.Outbox.Contracts;
using Common.Messaging.Outbox.Serialization;
using Common.Persistence.Transactions.Abstractions;

namespace Common.Messaging.Outbox.Transactions
{
    public sealed class OutboxTransactionParticipant(IOutboxEventBuffer eventBuffer, IOutboxMessageFactory messageFactory, IOutboxWriter writer) : ITransactionParticipant
    {
        public Task PrepareAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var events = eventBuffer.Drain();

            if(events.Count == 0)
                return Task.CompletedTask;

            var messages = events.Select(eventMessage => messageFactory.Create(eventMessage)).ToArray();

            writer.AddRange(messages);

            return Task.CompletedTask;
        }

        public Task CommittedAsync(CancellationToken cancellationToken = default)
        {
            eventBuffer.Clear();
            return Task.CompletedTask;
        }

        public Task AbortedAsync(CancellationToken cancellationToken = default)
        {
            eventBuffer.Clear();
            return Task.CompletedTask;
        }
    }
}
