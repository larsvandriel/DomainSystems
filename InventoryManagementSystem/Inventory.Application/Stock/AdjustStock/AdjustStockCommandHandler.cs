using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Stock.ApplyStockCount;
using Inventory.Application.Stock.Services;

namespace Inventory.Application.Stock.AdjustStock
{
    public sealed class AdjustStockCommandHandler(IResilientTransactionalExecutor transactionalExecutor) : IRequestHandler<AdjustStockCommand, Result>
    {
        public async Task<Result> HandleAsync(AdjustStockCommand request, CancellationToken cancellationToken = default)
        {
            var errors = Validate(request);

            if (errors.Any)
            {
                return Result.Failure(ProblemDetailsFactory.CreateValidation(
                    type: "error: InvalidAdjustStock",
                    detail: "One or more validation errors occurred.",
                    errors: errors.ToDictionary()));
            }

            var line = new StockCountLine(request.ItemId, request.ItemName, request.Amount, request.Unit);

            return await transactionalExecutor.ExecuteAsync<IStockMutationService>(
                (stockMutationService, ct) => stockMutationService.AdjustAsync(line, ct),
                cancellationToken);
        }

        private static ValidationErrors Validate(AdjustStockCommand request)
        {
            var errors = new ValidationErrors();

            if(request.ItemId == Guid.Empty)
                errors.Add(nameof(request.ItemId), "The stock count must have a given itemId.");

            if (string.IsNullOrWhiteSpace(request.ItemName))
                errors.Add(nameof(request.ItemName), "The stock count must have a given itemName.");

            if (request.Amount < 0)
                errors.Add(nameof(request.Amount), "The stock count must have a non-negative amount.");

            if (string.IsNullOrWhiteSpace(request.Unit))
                errors.Add(nameof(request.Unit), "The stock count must have a given unit.");

            return errors;
        }
    }
}
