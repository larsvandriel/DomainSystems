using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Stock.Services;

namespace Inventory.Application.Stock.DecreaseStock
{
    public sealed class DecreaseStockCommandHandler(IResilientTransactionExecutor transactionalExecutor) : IRequestHandler<DecreaseStockCommand, Result>
    {
        private readonly IResilientTransactionExecutor _transactionExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(DecreaseStockCommand request, CancellationToken cancellationToken = default)
        {
            return await _transactionExecutor.ExecuteAsync<IStockMutationService>((stockMutationService, ct) =>
                stockMutationService.DecreaseAsync(request.ItemId, request.ItemName, request.Amount, request.Unit, ct),
                cancellationToken);
        }
    }
}
