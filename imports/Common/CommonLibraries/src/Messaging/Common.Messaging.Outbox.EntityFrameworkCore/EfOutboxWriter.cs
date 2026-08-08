using Common.Messaging.Outbox.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Common.Messaging.Outbox.EntityFrameworkCore
{
    public sealed class EfOutboxWriter<TDbContext>(TDbContext dbContext) : IOutboxWriter where TDbContext: DbContext
    {
        public void Add(OutboxMessage message)
        {
            ArgumentNullException.ThrowIfNull(message);

            dbContext.Set<OutboxMessage>().Add(message);
        }

        public void AddRange(IReadOnlyCollection<OutboxMessage> messages)
        {
            ArgumentNullException.ThrowIfNull(messages);

            dbContext.Set<OutboxMessage>().AddRange(messages);
        }
    }
}
