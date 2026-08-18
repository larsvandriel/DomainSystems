using Common.Messaging.Abstractions.Validation;

namespace Inventory.Application.Reservations.ExpireReservation
{
    public sealed class ExpireReservationCommandValidator : IRequestValidator<ExpireReservationCommand>
    {
        public ValueTask<ValidationResult> ValidateAsync(ExpireReservationCommand request, CancellationToken cancellationToken = default)
        {
            var result = new ValidationResult();

            if (request.ReservationId == Guid.Empty)
                result.Add(nameof(request.ReservationId), "No reservationId was given when expiring reservation.");

            return ValueTask.FromResult(result);
        }
    }
}
