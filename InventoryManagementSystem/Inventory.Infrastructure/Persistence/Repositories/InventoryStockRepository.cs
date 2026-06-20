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
    public class InventoryStockRepository(InventoryDbContext dbContext, IRetryPolicy retryPolicy) : IInventoryStockRepository
    {
        private readonly InventoryDbContext _dbContext = dbContext;
        private readonly IRetryPolicy _retryPolicy = retryPolicy;

        public Task AddAsync(InventoryStock stock, CancellationToken cancellationToken)
        {
            _dbContext.InventoryStocks.Add(stock.ToEntity());
            return Task.CompletedTask;
        }

        public async Task UpdateAsync(InventoryStock stock, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.InventoryStocks.Include(x => x.Item).FirstOrDefaultAsync(x => x.ItemId == stock.Item.Id, cancellationToken)
                ?? throw new InvalidOperationException("Inventory stock not found.");

            entity.UpdateFromDomain(stock);
        }

        public async Task<InventoryStock?> GetByItemIdAsync(Guid itemId, CancellationToken cancellationToken)
        {
            var result = await _retryPolicy.ExecuteAsync(
                ct => _dbContext.InventoryStocks.AsNoTracking().Include(x => x.Item).FirstOrDefaultAsync(x => x.Item.Id == itemId, ct),
                cancellationToken);

            return result?.ToDomain();
        }

        public async Task<IReadOnlyList<InventoryStock>> GetAllAsync(CancellationToken cancellationToken)
        {
            var result = await _retryPolicy.ExecuteAsync(
                async ct => await _dbContext.InventoryStocks.AsNoTracking().Include(x => x.Item).ToListAsync(ct),
                cancellationToken);

            return [.. result.Select(x => x.ToDomain())];
        }
    }
}
