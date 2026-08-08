using Common.Messaging.Integration.Contracts;
using Common.Messaging.Outbox.Contracts;

namespace Common.Messaging.Outbox.Transactions
{
    public sealed class OutboxEventBuffer : IOutboxEventCollector, IOutboxEventBuffer
    {
        private readonly List<IIntegrationEvent> _events = [];

        public void Add(IIntegrationEvent integrationEvent)
        {
            ArgumentNullException.ThrowIfNull(integrationEvent);
            _events.Add(integrationEvent);
        }

        public IReadOnlyCollection<IIntegrationEvent> Drain()
        {
            if (_events.Count == 0)
            {
                return [];
            }

            var events = _events.ToArray();
            _events.Clear();

            return events;
        }

        public void Clear()
        {
            _events.Clear();
        }
    }
}
