using Common.Results;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Reservations.Services
{
    public interface IReservationQueryService
    {
        Task<Result<IReadOnlyList<InventoryReservation>>> GetAsync(ReservationQueryFilter filter, CancellationToken cancellationToken = default);
    }
}
