using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Stock.ApplyStockCount;
using Inventory.Application.Stock.Services;

namespace Inventory.Application.Stock.AdjustStock
{
    public sealed class AdjustStockCommandHandler(IResilientTransactionExecutor transactionalExecutor) : IRequestHandler<AdjustStockCommand, Result>
    {
        public async Task<Result> HandleAsync(AdjustStockCommand request, CancellationToken cancellationToken = default)
        {
            var line = new StockCountLine(request.ItemId, request.ItemName, request.Amount, request.Unit);

            return await transactionalExecutor.ExecuteAsync<IStockMutationService>(
                (stockMutationService, ct) => stockMutationService.AdjustAsync(line, ct),
                cancellationToken);
        }
    }
}
