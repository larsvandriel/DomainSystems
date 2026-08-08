using Common.Messaging.Abstractions.Requests;
using Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Reservations.CancelReservation
{
    public sealed record CancelReservationCommand(Guid ReservationId) : IRequest<Result>;
}