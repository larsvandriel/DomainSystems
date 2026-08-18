using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Reservations.Services;

namespace Inventory.Application.Reservations.CommitReservation
{
    public sealed class CommitReservationCommandHandler(
        IResilientTransactionExecutor transactionalExecutor
        ) : IRequestHandler<CommitReservationCommand, Result>
    {
        private readonly IResilientTransactionExecutor _transactionalExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(CommitReservationCommand request, CancellationToken cancellationToken = default)
        {
            return await _transactionalExecutor.ExecuteAsync<IReservationService>(
                (reservationService, ct) => reservationService.CommitReservationAsync(request.ReservationId, ct),
                cancellationToken);
        }
    }
}
