using Common.Resilience;
using Common.Persistence.Concurrency;
using Inventory.Application.Abstractions;
using Inventory.Domain.Models;
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

        public async Task AddAsync(InventoryStock stock, CancellationToken cancellationToken)
        {
            await _dbContext.InventoryStocks.AddAsync(stock.ToEntity(), cancellationToken);
        }

        public async Task UpdateAsync(InventoryStock stock, ConcurrencyToken concurrencyToken, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(stock);
            ArgumentNullException.ThrowIfNull(concurrencyToken);

            var entity = await _dbContext.InventoryStocks.Include(x => x.Item).FirstOrDefaultAsync(x => x.ItemId == stock.Item.Id, cancellationToken)
                ?? throw new InvalidOperationException($"Inventory stock for item '{stock.Item.Id}' was not found.");

            entity.UpdateFromDomain(stock);

            _dbContext.Entry(entity).Property(x => x.RowVersion).OriginalValue = concurrencyToken.ToArray();
        }

        public async Task<ConcurrencySnapshot<InventoryStock>?> GetByItemIdAsync(Guid itemId, CancellationToken cancellationToken)
        {
            var entity = await _retryPolicy.ExecuteAsync(
                ct => _dbContext.InventoryStocks.AsNoTracking().Include(x => x.Item).FirstOrDefaultAsync(x => x.ItemId == itemId, ct),
                cancellationToken);

            if (entity == null)
                return null;

            return new ConcurrencySnapshot<InventoryStock>(entity.ToDomain(), new ConcurrencyToken(entity.RowVersion));
        }

        public async Task<IReadOnlyList<InventoryStock>> GetAllAsync(CancellationToken cancellationToken)
        {
            var result = await _retryPolicy.ExecuteAsync(
                ct => _dbContext.InventoryStocks.AsNoTracking().Include(x => x.Item).ToListAsync(ct),
                cancellationToken);

            return [.. result.Select(x => x.ToDomain())];
        }
    }
}
