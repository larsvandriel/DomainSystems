using Inventory.Application.Abstractions;
using Inventory.Domain.Models;
using Inventory.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    public sealed class InventoryMutationRepository(InventoryDbContext dbContext) : IInventoryMutationRepository
    {
        private readonly InventoryDbContext _dbContext = dbContext;

        public Task AddAsync(InventoryMutation mutation, CancellationToken cancellationToken)
        {
           _dbContext.InventoryMutations.Add(mutation.ToEntity());
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<InventoryMutation>> GetAllByItemIdAsync(Guid itemId, CancellationToken cancellationToken)
        {
            var result = await _dbContext.InventoryMutations
                .AsNoTracking()
                .Include(x => x.Item)
                .Where(x => x.ItemId == itemId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return [.. result.Select(x => x.ToDomain())];
        }
    }
}
