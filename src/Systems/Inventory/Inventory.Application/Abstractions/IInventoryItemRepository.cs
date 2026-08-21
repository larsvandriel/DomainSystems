using Inventory.Domain.Models;

namespace Inventory.Application.Abstractions
{
    public interface IInventoryItemRepository
    {
        Task AddAsync(InventoryItem item, CancellationToken cancellationToken);
        Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken cancellationToken);
    }
}
