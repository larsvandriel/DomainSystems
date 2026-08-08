using Common.Results;
using Inventory.Application.Reservations.Models;
using Inventory.Domain.Models;

namespace Inventory.Application.Reservations.Services
{
    public interface IReservationQueryService
    {
        Task<Result<IReadOnlyList<InventoryReservation>>> GetAsync(ReservationQueryFilter filter, CancellationToken cancellationToken = default);
    }
}
