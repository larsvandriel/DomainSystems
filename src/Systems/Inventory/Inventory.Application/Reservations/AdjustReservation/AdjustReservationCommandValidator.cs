using Common.Messaging.Abstractions.Validation;

namespace Inventory.Application.Reservations.AdjustReservation
{
    public sealed class AdjustReservationCommandValidator : IRequestValidator<AdjustReservationCommand>
    {
        public ValueTask<ValidationResult> ValidateAsync(
            AdjustReservationCommand request,
            CancellationToken cancellationToken = default)
        {
            var result = new ValidationResult();
            if (request.Unit is null && request.Amount is null && request.Reference is null && !request.ExpiresAt.IsSpecified)
            {
                result.Add(nameof(request), "No changes received.");
                return ValueTask.FromResult(result);
            }

            if ((request.Unit is null && request.Amount is not null) || (request.Unit is not null && request.Amount is null))
                result.Add(nameof(request), "To change the amount both the amount and the unit must be set.");

            if (request.ReservationId == Guid.Empty)
                result.Add(nameof(request.ReservationId), "ReservationId cannot be empty.");

            if (request.Unit is not null && string.IsNullOrWhiteSpace(request.Unit))
                result.Add(nameof(request.Unit), "Unit cannot be empty.");

            if (request.Amount is not null && request.Amount <= 0)
                result.Add(nameof(request.Amount), "Amount must be a positive number.");

            if (request.ExpiresAt.IsSpecified && request.ExpiresAt.Value is not null && request.ExpiresAt.Value <= DateTimeOffset.UtcNow)
                result.Add(nameof(request.ExpiresAt), "ExpiresAt should not be in the past.");

            return ValueTask.FromResult(result);
        }
    }
}
