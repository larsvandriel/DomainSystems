using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Inventory.Application.Stock.Services;

namespace Inventory.Application.Stock.ApplyStockCount
{
    public sealed class ApplyStockCountCommandHandler(IResilientTransactionalExecutor transactionalExecutor) : IRequestHandler<ApplyStockCountCommand, Result>
    {
        public async Task<Result> HandleAsync(ApplyStockCountCommand request, CancellationToken cancellationToken = default)
        {
            var errors = Validate(request);

            if (errors.Any)
            {
                return Result.Failure(ProblemDetailsFactory.CreateValidation(
                    type: "error:InvalidStockCount",
                    detail: "One or more validation errors occurred.",
                    errors: errors.ToDictionary()));
            }

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

        private static ValidationErrors Validate(ApplyStockCountCommand request)
        {
            var errors = new ValidationErrors();
            if (request.Lines is null || request.Lines.Count == 0)
            {
                errors.Add(nameof(request.Lines),"No stock count lines provided.");
                return errors;
            }

            if (request.Lines.Any(line => line.ItemId == Guid.Empty))
                errors.Add(nameof(request.Lines), "All stock count lines must have a given itemId.");

            if (request.Lines.Any(line => string.IsNullOrWhiteSpace(line.ItemName)))
                errors.Add(nameof(request.Lines), "All stock count lines must have a given itemName.");

            if (request.Lines.Any(line => line.CountedAmount < 0))
                errors.Add(nameof(request.Lines), "All stock count should have non-negative amounts.");

            if (request.Lines.Any(line => string.IsNullOrWhiteSpace(line.Unit)))
                errors.Add(nameof(request.Lines), "All stock count lines must have a given unit.");

            ValidateDuplicateItemIds(request.Lines, errors);
            return errors;
        }

        private static void ValidateDuplicateItemIds(IReadOnlyCollection<StockCountLine> lines, ValidationErrors errors)
        {
            foreach (var message in lines
                .GroupBy(line => line.ItemId)
                .Where(group => group.Count() > 1)
                .Select(group => $"ItemId '{group.Key}' occurs {group.Count()} times."))
            {
                errors.Add(nameof(ApplyStockCountCommand.Lines), message);
            }
        }
    }
}
