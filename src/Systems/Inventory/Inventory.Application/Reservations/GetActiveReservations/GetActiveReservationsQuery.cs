using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Reservations.GetActiveReservations
{
    public sealed record GetActiveReservationsQuery : IRequest<Result<IReadOnlyList<InventoryReservation>>>;
}
