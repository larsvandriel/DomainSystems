using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Reservations.Services;

namespace Inventory.Application.Reservations.CancelReservation
{
    public sealed class CancelReservationCommandHandler(IResilientTransactionExecutor transactionalExecutor) : IRequestHandler<CancelReservationCommand, Result>
    {
        private readonly IResilientTransactionExecutor _transactionalExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(CancelReservationCommand request, CancellationToken cancellationToken = default)
        {
            return await _transactionalExecutor.ExecuteAsync<IReservationService>(
                (reservationService, ct) => reservationService.CancelReservationAsync(request.ReservationId, ct), cancellationToken);
        }
    }
}
