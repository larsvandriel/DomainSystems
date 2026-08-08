using Common.Messaging.Outbox.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Common.Messaging.Outbox.EntityFrameworkCore
{
    public sealed class EfOutboxStore<TDbContext>(TDbContext dbContext) : IOutboxStore where TDbContext : DbContext
    {
        public async Task<IReadOnlyCollection<OutboxMessage>> GetPendingAsync(int maximumCount, DateTimeOffset nowUtc, CancellationToken cancellationToken = default)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumCount);

            return await dbContext
                .Set<OutboxMessage>()
                .Where(message =>
                    message.PublishedAtUtc == null && message.DeadLetteredAtUtc == null && (message.NextAttemptAtUtc == null || message.NextAttemptAtUtc <= nowUtc))
                .OrderBy(message => message.NextAttemptAtUtc)
                .ThenBy(message => message.OccurredAtUtc)
                .ThenBy(message => message.Id)
                .Take(maximumCount)
                .ToArrayAsync(cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
