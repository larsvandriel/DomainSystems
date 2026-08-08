using Common.Messaging.Integration.Contracts;
using Common.Messaging.Outbox.Contracts;

namespace Common.Messaging.Outbox.Serialization
{
    public interface IOutboxMessageFactory
    {
        OutboxMessage Create(IIntegrationEvent integrationEvent, IntegrationEventContext? context = null);
    }
}
