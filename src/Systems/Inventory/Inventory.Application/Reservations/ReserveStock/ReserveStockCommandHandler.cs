using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Abstractions;
using Inventory.Application.Reservations.Services;
using Inventory.Application.Stock.Services;
using Inventory.Domain.Models;

namespace Inventory.Application.Reservations.ReserveStock
{
    public sealed class ReserveStockCommandHandler(IResilientTransactionalExecutor transactionalExecutor) : IRequestHandler<ReserveStockCommand, Result>
    {
        private readonly IResilientTransactionalExecutor _transactionalExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(ReserveStockCommand request, CancellationToken cancellationToken = default)
        {
            var errors = Validate(request);

            if (errors.Any)
                return Result.Failure(ProblemDetailsFactory.CreateValidation(
                    type: "error:InvalidReserveStock",
                    detail: "One or more validation errors occurred.",
                    errors: errors.ToDictionary()));

            var quantity = Quantity.Create(request.Amount, request.Unit);

            return await _transactionalExecutor.ExecuteAsync<IReservationService>(
                (reservationService, ct) => reservationService.CreateReservationAsync(request.ItemId, quantity, request.Reference, request.ExpiresAt, ct)
                , cancellationToken);
        }

        private static ValidationErrors Validate(ReserveStockCommand request)
        {
            var errors = new ValidationErrors();

            if (request.ItemId == Guid.Empty)
                errors.Add(nameof(request.ItemId), "ItemId cannot be empty.");

            if (string.IsNullOrWhiteSpace(request.Unit))
                errors.Add(nameof(request.Unit), "Unit cannot be empty.");

            if (request.Amount <= 0)
                errors.Add(nameof(request.Amount), "Amount must be a positive number.");

            if (request.ExpiresAt is not null && request.ExpiresAt <= DateTimeOffset.UtcNow)
                errors.Add(nameof(request.ExpiresAt), "ExpiresAt should not be in the past.");

            return errors;
        }
    }
}
