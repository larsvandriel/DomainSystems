using Inventory.Application.Abstractions;
using Inventory.Domain.Models;
using Inventory.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    public sealed class InventoryItemRepository(InventoryDbContext dbContext) : IInventoryItemRepository
    {
        public async Task AddAsync(InventoryItem item, CancellationToken cancellationToken)
        {
            await dbContext.InventoryItems.AddAsync(item.ToEntity(), cancellationToken);
        }

        public async Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken cancellationToken)
        {
            var result = await dbContext.InventoryItems.AsNoTracking().ToListAsync(cancellationToken);
            return [.. result.Select(x => x.ToDomain())];
        }

        public async Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await dbContext.InventoryItems.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);

            if(entity == null)
                return null;

            return entity.ToDomain();
        }
    }
}
