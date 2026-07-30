using Common.Persistence.Concurrency;
using Inventory.Application.Reservations.Services;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Abstractions
{
    public interface IInventoryReservationRepository
    {
        Task AddAsync(InventoryReservation reservation, CancellationToken cancellationToken);
        Task<InventoryReservation?> GetByReference(string reference, CancellationToken cancellationToken);
        Task<ConcurrencySnapshot<InventoryReservation>?> GetByIdAsync(Guid reservationId, CancellationToken cancellationToken);
        Task UpdateAsync(InventoryReservation reservation, ConcurrencyToken concurrencyToken, CancellationToken cancellationToken);
        Task<IReadOnlyList<InventoryReservation>> GetAsync(ReservationQueryFilter filter, DateTimeOffset activeAt, CancellationToken cancellationToken);
    }
}
