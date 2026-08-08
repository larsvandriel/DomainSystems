using Common.Persistence.Concurrency;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Abstractions
{
    public interface IInventoryReservationCapacityRepository
    {
        Task<ConcurrencySnapshot<InventoryReservationCapacity>?> GetByItemIdAsync(Guid itemId, CancellationToken cancellationToken);

        Task<IReadOnlyList<InventoryReservationCapacity>> GetAllAsync(CancellationToken cancellationToken);

        Task AddAsync(InventoryReservationCapacity capacity, CancellationToken cancellationToken);

        Task UpdateAsync(InventoryReservationCapacity capacity, ConcurrencyToken concurrencyToken, CancellationToken cancellationToken);
    }
}
