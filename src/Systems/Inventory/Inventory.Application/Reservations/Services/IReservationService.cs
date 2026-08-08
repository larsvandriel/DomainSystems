using Common.Optional;
using Common.Results;
using Inventory.Domain.Models;

namespace Inventory.Application.Reservations.Services
{
    public interface IReservationService
    {
        Task<Result> AdjustReservationAsync(Guid reservationId, Quantity? quantity, string? reference, Optional<DateTimeOffset?> expiresAt, CancellationToken cancellationToken);
        Task<Result> CommitReservationAsync(Guid reservationId, CancellationToken cancellationToken);
        Task<Result> ReleaseReservationAsync(Guid reservationId, CancellationToken cancellationToken);
        Task<Result> CancelReservationAsync(Guid reservationId, CancellationToken cancellationToken);
        Task<Result> ExpireReservationAsync(Guid reservationId, DateTimeOffset now, CancellationToken cancellationToken);
        Task<Result> CreateReservationAsync(Guid itemId, Quantity quantity, string? reference, DateTimeOffset? expiresAt, CancellationToken cancellationToken);
    }
}
