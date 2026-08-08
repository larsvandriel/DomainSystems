using Common.Messaging.Abstractions.Requests;
using Common.Results;

namespace Inventory.Application.Reservations.ExpireReservation
{
    public sealed record ExpireReservationCommand(Guid ReservationId) : IRequest<Result>;
}
