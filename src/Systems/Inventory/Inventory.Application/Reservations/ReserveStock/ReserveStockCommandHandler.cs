using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Reservations.Services;
using Inventory.Domain.Models;

namespace Inventory.Application.Reservations.ReserveStock
{
    public sealed class ReserveStockCommandHandler(IResilientTransactionExecutor transactionalExecutor) : IRequestHandler<ReserveStockCommand, Result>
    {
        private readonly IResilientTransactionExecutor _transactionalExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(ReserveStockCommand request, CancellationToken cancellationToken = default)
        {
            var quantity = Quantity.Create(request.Amount, request.Unit);

            return await _transactionalExecutor.ExecuteAsync<IReservationService>(
                (reservationService, ct) => reservationService.CreateReservationAsync(request.ItemId, quantity, request.Reference, request.ExpiresAt, ct)
                , cancellationToken);
        }
    }
}
