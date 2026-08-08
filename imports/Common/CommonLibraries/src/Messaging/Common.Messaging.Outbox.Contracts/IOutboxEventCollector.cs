using Common.Messaging.Integration.Contracts;

namespace Common.Messaging.Outbox.Contracts
{
    public interface IOutboxEventCollector
    {
        void Add(IIntegrationEvent integrationEvent);
    }
}
