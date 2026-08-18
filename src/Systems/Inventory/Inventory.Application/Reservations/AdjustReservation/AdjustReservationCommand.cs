using Common.Messaging.Abstractions.Requests;
using Common.Optional;
using Common.Results;

namespace Inventory.Application.Reservations.AdjustReservation
{
    public sealed record AdjustReservationCommand(Guid ReservationId, decimal? Amount, string? Unit, string? Reference, Optional<DateTimeOffset?> ExpiresAt) : IRequest<Result>;
}
