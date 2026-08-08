using Common.Messaging.Integration.Contracts;

namespace Common.Messaging.Outbox.Contracts
{
    public interface IOutboxTransport
    {
        Task PublishAsync(IntegrationEventEnvelope envelope, CancellationToken cancellationToken = default);
    }
}
