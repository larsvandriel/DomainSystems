using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Reservations.Services;

namespace Inventory.Application.Reservations.ReleaseReservation
{
    public sealed class ReleaseReservationCommandHandler(IResilientTransactionalExecutor transactionalExecutor) : IRequestHandler<ReleaseReservationCommand, Result>
    {
        private readonly IResilientTransactionalExecutor _transactionalExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(ReleaseReservationCommand request, CancellationToken cancellationToken = default)
        {
            var errors = Validate(request);

            if (errors.Any)
                return Result.Failure(ProblemDetailsFactory.CreateValidation(
                    type: "error:InvalidReleaseStock",
                    detail: "One or more validation errors occurred.",
                    errors: errors.ToDictionary()));

            return await _transactionalExecutor.ExecuteAsync<IReservationService>(
                (reservationService, ct) => reservationService.ReleaseReservationAsync(request.ReservationId, ct), cancellationToken);
        }

        private static ValidationErrors Validate(ReleaseReservationCommand request)
        {
            var errors = new ValidationErrors();

            if (request.ReservationId == Guid.Empty)
                errors.Add(nameof(request.ReservationId), "No reservationId was given when releasing reservation.");

            return errors;
        }
    }
}
