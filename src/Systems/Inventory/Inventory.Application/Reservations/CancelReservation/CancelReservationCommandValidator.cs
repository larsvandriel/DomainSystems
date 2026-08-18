using Common.Messaging.Abstractions.Validation;

namespace Inventory.Application.Reservations.CancelReservation
{
    public sealed class CancelReservationCommandValidator : IRequestValidator<CancelReservationCommand>
    {
        public ValueTask<ValidationResult> ValidateAsync(
            CancelReservationCommand request,
            CancellationToken cancellationToken = default)
        {
            var result = new ValidationResult();

            if (request.ReservationId == Guid.Empty)
                result.Add(nameof(request.ReservationId), "No reservationId was given when cancelling reservation.");

            return ValueTask.FromResult(result);
        }
    }
}
