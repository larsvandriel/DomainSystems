using Common.Messaging.Abstractions.Requests;
using Common.Results;

namespace Inventory.Application.Reservations.CommitReservation
{
    public sealed record CommitReservationCommand(Guid ReservationId) : IRequest<Result>;
}
