using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Application.Reservations.Enums;
using Inventory.Application.Reservations.Services;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Reservations.GetActiveReservations
{
    public sealed class GetActiveReservationsQueryHandler(
        IReservationQueryService reservationQueryService) : IRequestHandler<GetActiveReservationsQuery, Result<IReadOnlyList<InventoryReservation>>>
    {
        private readonly IReservationQueryService _reservationQueryService = reservationQueryService;

        public async Task<Result<IReadOnlyList<InventoryReservation>>> HandleAsync(GetActiveReservationsQuery request, CancellationToken cancellationToken = default)
        {
            return await _reservationQueryService.GetAsync(new ReservationQueryFilter(Selection: ReservationSelection.Active), cancellationToken);
        }
    }
}
