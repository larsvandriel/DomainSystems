using Common.Resilience;
using Inventory.Application.Stock.Abstractions;
using Inventory.Domain;
using Inventory.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    public sealed class InventoryMutationRepository(InventoryDbContext dbContext, IRetryPolicy retryPolicy) : IInventoryMutationRepository
    {
        private readonly InventoryDbContext _dbContext = dbContext;
        private readonly IRetryPolicy _retryPolicy = retryPolicy;

        public Task AddAsync(InventoryMutation mutation, CancellationToken cancellationToken)
        {
           _dbContext.InventoryMutations.Add(mutation.ToEntity());
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<InventoryMutation>> GetAllByItemIdAsync(Guid itemId, CancellationToken cancellationToken)
        {
            var result = await _retryPolicy.ExecuteAsync(
                async ct => await _dbContext.InventoryMutations
                    .AsNoTracking()
                    .Include(x => x.Item)
                    .Where(x => x.ItemId == itemId)
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync(ct),
                cancellationToken);

            return [.. result.Select(x => x.ToDomain())];
        }
    }
}
