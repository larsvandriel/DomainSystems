using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Reservations.Services;

namespace Inventory.Application.Reservations.ExpireReservation
{
    public sealed class ExpireReservationCommandHandler(IResilientTransactionalExecutor transactionalExecutor) : IRequestHandler<ExpireReservationCommand, Result>
    {
        private readonly IResilientTransactionalExecutor _transactionalExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(ExpireReservationCommand request, CancellationToken cancellationToken = default)
        {
            var errors = Validate(request);

            if (errors.Any)
                return Result.Failure(ProblemDetailsFactory.CreateValidation(
                    type: "error:InvalidExpireReservation",
                    detail: "One or more validation errors occurred.",
                    errors: errors.ToDictionary()));

            return await _transactionalExecutor.ExecuteAsync<IReservationService>(
                (reservationService, ct) => reservationService.ExpireReservationAsync(request.ReservationId, DateTimeOffset.UtcNow, ct), cancellationToken);
        }

        private static ValidationErrors Validate(ExpireReservationCommand request)
        {
            var errors = new ValidationErrors();

            if (request.ReservationId == Guid.Empty)
                errors.Add(nameof(request.ReservationId), "No reservationId was given when expiring reservation.");

            return errors;
        }
    }
}
