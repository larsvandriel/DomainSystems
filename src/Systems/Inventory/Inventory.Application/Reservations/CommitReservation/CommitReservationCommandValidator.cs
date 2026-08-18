using Common.Messaging.Abstractions.Validation;

namespace Inventory.Application.Reservations.CommitReservation
{
    public sealed class CommitReservationCommandValidator : IRequestValidator<CommitReservationCommand>
    {
        public ValueTask<ValidationResult> ValidateAsync(
            CommitReservationCommand request,
            CancellationToken cancellationToken = default)
        {
            var result = new ValidationResult();

            if (request.ReservationId == Guid.Empty)
                result.Add(nameof(request.ReservationId), "No reservationId was given when committing reservation.");

            return ValueTask.FromResult(result);
        }
    }
}
