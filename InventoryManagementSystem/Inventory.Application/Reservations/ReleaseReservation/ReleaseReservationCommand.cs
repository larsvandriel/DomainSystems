using Common.Messaging.Abstractions.Requests;
using Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Reservations.ReleaseReservation
{
    public sealed record ReleaseReservationCommand(Guid ReservationId) : IRequest<Result>;
}
