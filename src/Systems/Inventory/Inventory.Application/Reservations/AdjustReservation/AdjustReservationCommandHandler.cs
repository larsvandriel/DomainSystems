using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Reservations.Services;
using Inventory.Domain.Models;

namespace Inventory.Application.Reservations.AdjustReservation
{
    public sealed class AdjustReservationCommandHandler(IResilientTransactionExecutor transactionalExecutor) : IRequestHandler<AdjustReservationCommand, Result>
    {
        private readonly IResilientTransactionExecutor _transactionalExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(AdjustReservationCommand request, CancellationToken cancellationToken = default)
        {
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
    }
}
