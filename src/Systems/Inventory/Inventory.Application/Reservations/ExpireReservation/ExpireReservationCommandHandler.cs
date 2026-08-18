using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Reservations.Services;

namespace Inventory.Application.Reservations.ExpireReservation
{
    public sealed class ExpireReservationCommandHandler(IResilientTransactionExecutor transactionalExecutor) : IRequestHandler<ExpireReservationCommand, Result>
    {
        private readonly IResilientTransactionExecutor _transactionalExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(ExpireReservationCommand request, CancellationToken cancellationToken = default)
        {
            return await _transactionalExecutor.ExecuteAsync<IReservationService>(
                (reservationService, ct) => reservationService.ExpireReservationAsync(request.ReservationId, DateTimeOffset.UtcNow, ct), cancellationToken);
        }
    }
}
