using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Abstractions;
using Common.Results;
using Inventory.Application.Services;
using Inventory.Application.Stock.ApplyStockCount;

namespace Inventory.Application.Stock.AdjustStock
{
    public sealed class AdjustStockCommandHandler(
        ITransactionalExecutor transactionalExecutor,
        IStockAdjustmentService adjustmentService)
        : IRequestHandler<AdjustStockCommand, Result>
    {
        public Task<Result> HandleAsync(AdjustStockCommand request, CancellationToken cancellationToken = default)
        {
            return transactionalExecutor.ExecuteAsync(async ct =>
            {
                await adjustmentService.ApplyAsync(
                    new StockCountLine(
                        request.ItemId,
                        request.ItemName,
                        request.Amount,
                        request.Unit),
                ct);

                return Result.Success();
            }, cancellationToken);
        }
    }
}
