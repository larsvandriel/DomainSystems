using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Reservations.GetReservations
{
    public sealed record GetReservationsQuery() : IRequest<Result<IReadOnlyList<InventoryReservation>>>;
}
