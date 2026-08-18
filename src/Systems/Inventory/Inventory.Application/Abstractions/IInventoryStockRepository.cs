using Common.Persistence.Concurrency;
using Inventory.Domain.Models;

namespace Inventory.Application.Abstractions
{
    public interface IInventoryStockRepository
    {
        Task AddAsync(InventoryStock stock, CancellationToken cancellationToken);
        Task<IReadOnlyList<InventoryStock>> GetAllAsync(CancellationToken cancellationToken);
        Task<ConcurrencySnapshot<InventoryStock>?> GetByItemIdAsync(Guid itemId, CancellationToken cancellationToken);
        Task UpdateAsync(InventoryStock stock, ConcurrencyToken concurrencyToken, CancellationToken cancellationToken);
    }
}
