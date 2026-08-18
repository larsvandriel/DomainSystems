using Inventory.Domain.Models;

namespace Inventory.Application.Abstractions
{
    public interface IInventoryMutationRepository
    {
        Task AddAsync(InventoryMutation mutation, CancellationToken cancellationToken);
        Task<IReadOnlyList<InventoryMutation>> GetAllByItemIdAsync(Guid itemId, CancellationToken cancellationToken);
    }
}
