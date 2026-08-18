using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Stock.Services;

namespace Inventory.Application.Stock.IncreaseStock
{
    public sealed class IncreaseStockCommandHandler(IResilientTransactionExecutor transactionalExecutor) : IRequestHandler<IncreaseStockCommand, Result>
    {
        private readonly IResilientTransactionExecutor _transactionalExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(IncreaseStockCommand request, CancellationToken cancellationToken = default)
        {
            return await _transactionalExecutor.ExecuteAsync<IStockMutationService>((stockMutationService, ct) =>
            stockMutationService.IncreaseAsync(request.ItemId, request.ItemName, request.Amount, request.Unit, ct),
            cancellationToken);
        }
    }
}
