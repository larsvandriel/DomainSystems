using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Stock.Services;

namespace Inventory.Application.Stock.ApplyStockCount
{
    public sealed class ApplyStockCountCommandHandler(IResilientTransactionExecutor transactionalExecutor) : IRequestHandler<ApplyStockCountCommand, Result>
    {
        public async Task<Result> HandleAsync(ApplyStockCountCommand request, CancellationToken cancellationToken = default)
        {
            return await transactionalExecutor.ExecuteAsync<IStockMutationService>(async (stockMutationService, ct) =>
            {
                foreach (var line in request.Lines)
                {
                    var result = await stockMutationService.AdjustAsync(line, ct);
                    if (result.IsFailure)
                        return result;
                }

                return Result.Success();
            }, cancellationToken);
        }
    }
}
