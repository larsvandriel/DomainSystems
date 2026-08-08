using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Stock.Services;

namespace Inventory.Application.Stock.IncreaseStock
{
    public sealed class IncreaseStockCommandHandler(IResilientTransactionalExecutor transactionalExecutor) : IRequestHandler<IncreaseStockCommand, Result>
    {
        private readonly IResilientTransactionalExecutor _transactionalExecutor = transactionalExecutor;

        public async Task<Result> HandleAsync(IncreaseStockCommand request, CancellationToken cancellationToken = default)
        {
            var validationErrors = Validate(request);

            if (validationErrors.Any)
            {
                return Result.Failure(ProblemDetailsFactory.CreateValidation(
                    "error:InvalidIncreaseStock",
                    "One or more validation errors occurred.",
                    validationErrors.ToDictionary()));
            }

            return await _transactionalExecutor.ExecuteAsync<IStockMutationService>((stockMutationService, ct) =>
            stockMutationService.IncreaseAsync(request.ItemId, request.ItemName, request.Amount, request.Unit, ct),
            cancellationToken);
        }

        public static ValidationErrors Validate(IncreaseStockCommand request)
        {
            var errors = new ValidationErrors();
            if (request.ItemId == Guid.Empty)
            {
                errors.Add(nameof(request.ItemId), "ItemId must not be empty.");
            }
            if (string.IsNullOrWhiteSpace(request.ItemName))
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
