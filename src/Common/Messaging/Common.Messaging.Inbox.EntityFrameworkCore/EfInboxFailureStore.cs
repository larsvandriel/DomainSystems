using Common.Messaging.Inbox.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Common.Messaging.Inbox.EntityFrameworkCore
{
    public sealed class EfInboxFailureStore<TDbContext>(TDbContext dbContext) : IInboxFailureStore where TDbContext: DbContext
    {
        public void Add(InboxFailure failure)
        {
            ArgumentNullException.ThrowIfNull(failure);

            dbContext.Set<InboxFailure>().Add(failure);
        }
    }
}
