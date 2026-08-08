using Common.Messaging.Inbox.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Common.Messaging.Inbox.EntityFrameworkCore
{
    public sealed class EfInboxStore<TDbContext>(TDbContext dbContext) : IInboxStore where TDbContext: DbContext
    {
        public Task<InboxMessage?> FindAsync(Guid messageId, string consumer, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(consumer);

            return dbContext.Set<InboxMessage>().SingleOrDefaultAsync(
                message => message.MessageId == messageId && message.Consumer == consumer, cancellationToken);
        }

        public void Add(InboxMessage message)
        {
            ArgumentNullException.ThrowIfNull(message);

            dbContext.Set<InboxMessage>().Add(message);
        }
    }
}
