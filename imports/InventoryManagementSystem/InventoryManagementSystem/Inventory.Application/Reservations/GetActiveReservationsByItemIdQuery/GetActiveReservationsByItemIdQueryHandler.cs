using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Application.Reservations.Enums;
using Inventory.Application.Reservations.Models;
using Inventory.Application.Reservations.Services;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Reservations.GetActiveReservationsByItemIdQuery
{
    public sealed class GetActiveReservationsByItemIdQueryHandler(
        IReservationQueryService reservationQueryService) : IRequestHandler<GetActiveReservationsByItemIdQuery, Result<IReadOnlyList<InventoryReservation>>>
    {
        private readonly IReservationQueryService _reservationQueryService = reservationQueryService;

        public async Task<Result<IReadOnlyList<InventoryReservation>>> HandleAsync(GetActiveReservationsByItemIdQuery request, CancellationToken cancellationToken = default)
        {
            return await _reservationQueryService.GetAsync(new ReservationQueryFilter(ItemId: request.ItemId, Selection: ReservationSelection.Active), cancellationToken);
        }
    }
}
