using Inventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Abstractions
{
    public interface IInventoryStockRepository
    {
        Task AddAsync(InventoryStock stock, CancellationToken cancellationToken);
        Task<IReadOnlyList<InventoryStock>> GetAllAsync(CancellationToken cancellationToken);
        Task<InventoryStock?> GetByItemIdAsync(Guid itemId, CancellationToken cancellationToken);
        Task UpdateAsync(InventoryStock stock, CancellationToken cancellationToken);
    }
}
