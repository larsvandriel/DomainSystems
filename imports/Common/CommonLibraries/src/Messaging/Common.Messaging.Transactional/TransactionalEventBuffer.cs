using Common.Messaging.Abstractions.Events;

namespace Common.Messaging.Transactional
{
    public class TransactionalEventBuffer : ITransactionalEventCollector, ITransactionalEventBuffer
    {
        private readonly List<IEvent> _events = [];

        public void Add(IEvent eventMessage)
        {
            ArgumentNullException.ThrowIfNull(eventMessage);
            _events.Add(eventMessage);
        }

        public void Clear()
        {
            _events.Clear();
        }

        public IReadOnlyList<IEvent> TakeAll()
        {
            var events = _events.ToArray();
            _events.Clear();
            return events;
        }
    }
}
