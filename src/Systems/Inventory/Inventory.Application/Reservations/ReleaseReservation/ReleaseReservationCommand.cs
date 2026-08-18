using Common.Messaging.Abstractions.Requests;
using Common.Results;

namespace Inventory.Application.Reservations.ReleaseReservation
{
    public sealed record ReleaseReservationCommand(Guid ReservationId) : IRequest<Result>;
}
