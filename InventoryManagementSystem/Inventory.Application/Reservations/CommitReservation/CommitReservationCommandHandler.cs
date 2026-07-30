using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Abstractions;
using Inventory.Application.Reservations.Services;
using Inventory.Application.Stock.Services;
using Inventory.Domain.Enums;

namespace Inventory.Application.Reservations.CommitReservation
{
    public sealed class CommitReservationCommandHandler(
        IResilientTransactionalExecutor transactionalExecutor
        ) : IRequestHandler<CommitReservationCommand, Result>
    {
        private readonly IResilientTransactionalExecutor _transactionalExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(CommitReservationCommand request, CancellationToken cancellationToken = default)
        {
            var errors = Validate(request);

            if (errors.Any)
                return Result.Failure(ProblemDetailsFactory.CreateValidation(
                    type: "error:InvalidCommitReservation",
                    detail: "One or more validation errors occurred.",
                    errors: errors.ToDictionary()));

            return await _transactionalExecutor.ExecuteAsync<IReservationService>(
                (reservationService, ct) => reservationService.CommitReservationAsync(request.ReservationId, ct),
                cancellationToken);
        }

        private static ValidationErrors Validate(CommitReservationCommand request)
        {
            var errors = new ValidationErrors();

            if (request.ReservationId == Guid.Empty)
                errors.Add(nameof(request.ReservationId), "No reservationId was given when committing reservation.");

            return errors;
        }
    }
}
