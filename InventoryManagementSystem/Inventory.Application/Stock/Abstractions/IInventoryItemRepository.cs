using Inventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.Abstractions
{
    public interface IInventoryItemRepository
    {
        Task AddAsync(InventoryItem item, CancellationToken cancellationToken);
        Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken cancellationToken);
        Task UpdateAsync(InventoryItem item, CancellationToken cancellationToken);
    }
}
