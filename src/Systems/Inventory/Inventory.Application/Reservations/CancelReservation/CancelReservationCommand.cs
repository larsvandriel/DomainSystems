using Common.Messaging.Abstractions.Requests;
using Common.Results;

namespace Inventory.Application.Reservations.CancelReservation
{
    public sealed record CancelReservationCommand(Guid ReservationId) : IRequest<Result>;
}
