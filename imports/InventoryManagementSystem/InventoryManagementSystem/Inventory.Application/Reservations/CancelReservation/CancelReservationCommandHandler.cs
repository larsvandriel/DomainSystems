using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Reservations.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Reservations.CancelReservation
{
    public sealed class CancelReservationCommandHandler(IResilientTransactionalExecutor transactionalExecutor) : IRequestHandler<CancelReservationCommand, Result>
    {
        private readonly IResilientTransactionalExecutor _transactionalExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(CancelReservationCommand request, CancellationToken cancellationToken = default)
        {
            var errors = Validate(request);

            if (errors.Any)
                return Result.Failure(ProblemDetailsFactory.CreateValidation(
                    type: "error:InvalidCancelReservation",
                    detail: "One or more validation errors occurred.",
                    errors: errors.ToDictionary()));

            return await _transactionalExecutor.ExecuteAsync<IReservationService>(
                (reservationService, ct) => reservationService.CancelReservationAsync(request.ReservationId, ct), cancellationToken);
        }

        private static ValidationErrors Validate(CancelReservationCommand request)
        {
            var errors = new ValidationErrors();

            if (request.ReservationId == Guid.Empty)
                errors.Add(nameof(request.ReservationId), "No reservationId was given when cancelling reservation.");

            return errors;
        }
    }
}
