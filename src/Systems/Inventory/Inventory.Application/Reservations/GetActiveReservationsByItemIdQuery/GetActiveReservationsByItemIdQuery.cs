using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Reservations.GetActiveReservationsByItemIdQuery
{
    public sealed record GetActiveReservationsByItemIdQuery(Guid ItemId) : IRequest<Result<IReadOnlyList<InventoryReservation>>>;
}
