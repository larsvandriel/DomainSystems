using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Stock.Services;

namespace Inventory.Application.Stock.DecreaseStock
{
    public sealed class DecreaseStockCommandHandler(IResilientTransactionalExecutor transactionalExecutor) : IRequestHandler<DecreaseStockCommand, Result>
    {
        private readonly IResilientTransactionalExecutor _transactionExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(DecreaseStockCommand request, CancellationToken cancellationToken = default)
        {
            var validationErrors = Validate(request);

            if (validationErrors.Any)
            {
                return Result.Failure(ProblemDetailsFactory.CreateValidation(
                    "error:InvalidDecreaseStock",
                    "One or more validation errors occurred.",
                    validationErrors.ToDictionary()));
            }
            
            return await _transactionExecutor.ExecuteAsync<IStockMutationService>((stockMutationService, ct) =>
                stockMutationService.DecreaseAsync(request.ItemId, request.ItemName, request.Amount, request.Unit, ct),
                cancellationToken);
        }

        private static ValidationErrors Validate(DecreaseStockCommand request)
        {
            var errors = new ValidationErrors();
            if(request.ItemId == Guid.Empty)
            {
                errors.Add(nameof(request.ItemId), "ItemId must not be empty.");
            }
            if(string.IsNullOrWhiteSpace(request.ItemName))
            {
                errors.Add(nameof(request.ItemName), "ItemName must not be empty.");
            }
            if (request.Amount <= 0)
            {
                errors.Add(nameof(request.Amount), "Amount must be greater than zero.");
            }
            if (string.IsNullOrWhiteSpace(request.Unit))
            {
                errors.Add(nameof(request.Unit), "Unit must not be empty.");
            }
            return errors;
        }
    }
}
