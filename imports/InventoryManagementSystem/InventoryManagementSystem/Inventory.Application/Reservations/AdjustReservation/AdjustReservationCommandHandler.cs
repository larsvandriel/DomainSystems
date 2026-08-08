using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Abstractions;
using Inventory.Application.Reservations.Services;
using Inventory.Application.Stock.Services;
using Inventory.Domain.Models;

namespace Inventory.Application.Reservations.AdjustReservation
{
    public sealed class AdjustReservationCommandHandler(IResilientTransactionalExecutor transactionalExecutor) : IRequestHandler<AdjustReservationCommand, Result>
    {
        private readonly IResilientTransactionalExecutor _transactionalExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(AdjustReservationCommand request, CancellationToken cancellationToken = default)
        {
            var errors = Validate(request);

            if (errors.Any)
                return Result.Failure(ProblemDetailsFactory.CreateValidation(
                    type: "error:InvalidAdjustReservation",
                    detail: "One or more validation errors occurred.",
                    errors: errors.ToDictionary()));

            Quantity? quantity = null;

            if (request.Unit is not null && request.Amount is not null)
            {
                quantity = Quantity.Create(request.Amount.Value, request.Unit);
            }

            return await _transactionalExecutor.ExecuteAsync<IReservationService>(
                (reservationService, ct) => reservationService.AdjustReservationAsync(
                    request.ReservationId, quantity, request.Reference, request.ExpiresAt, ct),
                cancellationToken);
        }

        private static ValidationErrors Validate(AdjustReservationCommand request)
        {
            var errors = new ValidationErrors();

            if (request.Unit is null && request.Amount is null && request.Reference is null && !request.ExpiresAt.IsSpecified)
            {
                errors.Add(nameof(request), "No changes received.");
                return errors;
            }

            if ((request.Unit is null && request.Amount is not null) || (request.Unit is not null && request.Amount is null))
                errors.Add(nameof(request), "To change the amount both the amount and the unit must be set.");

            if (request.ReservationId == Guid.Empty)
                errors.Add(nameof(request.ReservationId), "ReservationId cannot be empty.");

            if (request.Unit is not null && string.IsNullOrWhiteSpace(request.Unit))
                errors.Add(nameof(request.Unit), "Unit cannot be empty.");

            if (request.Amount is not null && request.Amount <= 0)
                errors.Add(nameof(request.Amount), "Amount must be a positive number.");

            if (request.ExpiresAt.IsSpecified && request.ExpiresAt.Value is not null && request.ExpiresAt.Value <= DateTimeOffset.UtcNow)
                errors.Add(nameof(request.ExpiresAt), "ExpiresAt should not be in the past.");

            return errors;
        }
    }
}
