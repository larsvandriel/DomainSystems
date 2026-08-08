using Common.Messaging.Integration.Contracts;
using Common.Messaging.Outbox.Contracts;

namespace Common.Messaging.Outbox.Transactions
{
    public interface IOutboxEventBuffer
    {
        IReadOnlyCollection<IIntegrationEvent> Drain();

        void Clear();
    }
}
