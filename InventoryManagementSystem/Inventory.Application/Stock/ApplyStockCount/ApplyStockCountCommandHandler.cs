using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Abstractions;
using Common.Results;
using Inventory.Application.Services;

namespace Inventory.Application.Stock.ApplyStockCount
{
    public sealed class ApplyStockCountCommandHandler(
        ITransactionalExecutor transactionalExecutor,
        IStockAdjustmentService adjustmentService)
        : IRequestHandler<ApplyStockCountCommand, Result>
    {
        public Task<Result> HandleAsync(ApplyStockCountCommand request, CancellationToken cancellationToken = default)
        {
            return transactionalExecutor.ExecuteAsync(async ct =>
            {
                if (request.Lines is null || request.Lines.Count == 0)
                    return Result.Failure("No stock count lines provided.");

                foreach (var line in request.Lines)
                {
                    await adjustmentService.ApplyAsync(line, ct);
                }

                return Result.Success();
            }, cancellationToken);
        }
    }
}
