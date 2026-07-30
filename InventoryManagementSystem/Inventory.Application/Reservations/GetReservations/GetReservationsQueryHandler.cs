using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Application.Reservations.Services;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Reservations.GetReservations
{
    public sealed class GetReservationsQueryHandler(
        IReservationQueryService reservationQueryService) : IRequestHandler<GetReservationsQuery, Result<IReadOnlyList<InventoryReservation>>>
    {
        private readonly IReservationQueryService _reservationQueryService = reservationQueryService;

        public async Task<Result<IReadOnlyList<InventoryReservation>>> HandleAsync(GetReservationsQuery request, CancellationToken cancellationToken = default)
        {
            return await _reservationQueryService.GetAsync(new ReservationQueryFilter(), cancellationToken);
        }
    }
}
