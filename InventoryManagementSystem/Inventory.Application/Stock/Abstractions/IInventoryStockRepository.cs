using Inventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.Abstractions
{
    public interface IInventoryStockRepository
    {
        Task AddAsync(object stock, CancellationToken cancellationToken);
        Task<IEnumerable<InventoryStock>> GetAllAsync(CancellationToken cancellationToken);
        Task<InventoryStock> GetByItemIdAsync(Guid itemId, CancellationToken cancellationToken);
        Task UpdateAsync(object stock, CancellationToken cancellationToken);
    }
}
