using Common.Messaging.Integration.Contracts;
using Common.Messaging.Outbox.Contracts;
using System.Reflection;
using System.Text.Json;

namespace Common.Messaging.Outbox.Serialization
{
    public sealed class JsonOutboxMessageFactory(JsonSerializerOptions serializerOptions, TimeProvider timeProvider) : IOutboxMessageFactory
    {
        public OutboxMessage Create(IIntegrationEvent integrationEvent, IntegrationEventContext? context = null)
        {
            ArgumentNullException.ThrowIfNull(integrationEvent);

            var eventType = integrationEvent.GetType();

            var metadata = eventType.GetCustomAttribute<IntegrationEventTypeAttribute>()
                ?? throw new InvalidOperationException(
                    $"Integration event type '{eventType.FullName}' is missing the required '{nameof(IntegrationEventTypeAttribute)}' attribute.");

            var payload = JsonSerializer.Serialize(integrationEvent, eventType, serializerOptions);

            return OutboxMessage.Create(
                id: Guid.NewGuid(),
                occurredAtUtc: timeProvider.GetUtcNow(),
                type: metadata.Name,
                version: metadata.Version,
                payload: payload,
                correlationId: context?.CorrelationId,
                causationId: context?.CausationId,
                traceId: context?.TraceId);
        }
    }
}
