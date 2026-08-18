using Common.Persistence.Concurrency;
using Inventory.Domain.Models;

namespace Inventory.Application.Abstractions
{
    public interface IInventoryReservationRepository
    {
        Task AddAsync(InventoryReservation reservation, CancellationToken cancellationToken);
        Task<InventoryReservation?> GetByReference(string reference, CancellationToken cancellationToken);
        Task<ConcurrencySnapshot<InventoryReservation>?> GetByIdAsync(Guid reservationId, CancellationToken cancellationToken);
        Task UpdateAsync(InventoryReservation reservation, ConcurrencyToken concurrencyToken, CancellationToken cancellationToken);
    }
}
