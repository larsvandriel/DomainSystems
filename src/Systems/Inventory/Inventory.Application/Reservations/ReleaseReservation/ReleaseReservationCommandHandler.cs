using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Reservations.Services;

namespace Inventory.Application.Reservations.ReleaseReservation
{
    public sealed class ReleaseReservationCommandHandler(IResilientTransactionExecutor transactionalExecutor) : IRequestHandler<ReleaseReservationCommand, Result>
    {
        private readonly IResilientTransactionExecutor _transactionalExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(ReleaseReservationCommand request, CancellationToken cancellationToken = default)
        {
            return await _transactionalExecutor.ExecuteAsync<IReservationService>(
                (reservationService, ct) => reservationService.ReleaseReservationAsync(request.ReservationId, ct), cancellationToken);
        }
    }
}
