using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Reservations.GetReservationsByItemId
{
    public sealed record GetReservationsByItemIdQuery(Guid ItemId) : IRequest<Result<IReadOnlyList<InventoryReservation>>>
    {
    }
}
