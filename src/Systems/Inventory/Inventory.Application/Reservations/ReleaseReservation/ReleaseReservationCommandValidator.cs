using Common.Messaging.Abstractions.Validation;

namespace Inventory.Application.Reservations.ReleaseReservation
{
    public sealed class ReleaseReservationCommandValidator : IRequestValidator<ReleaseReservationCommand>
    {
        public ValueTask<ValidationResult> ValidateAsync(ReleaseReservationCommand request, CancellationToken cancellationToken = default)
        {
            var result = new ValidationResult();

            if (request.ReservationId == Guid.Empty)
                result.Add(nameof(request.ReservationId), "No reservationId was given when releasing reservation.");

            return ValueTask.FromResult(result);
        }
    }
}
