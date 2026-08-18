using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Common.Results.Problems;
using Inventory.Application.Abstractions;
using Inventory.Domain.Models;

namespace Inventory.Application.Stock.GetStock
{
    public sealed class GetStockQueryHandler(IInventoryStockRepository stockRepository) : IRequestHandler<GetStockQuery, Result<InventoryStock>>
    {
        private readonly IInventoryStockRepository _stockRepository = stockRepository;
        
        public async Task<Result<InventoryStock>> HandleAsync(GetStockQuery request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var snapshot = await _stockRepository.GetByItemIdAsync(request.ItemId, cancellationToken);

            if(snapshot is null)
            {
                return Result.Failure<InventoryStock>(
                    ProblemFactory.NotFound("error: InventoryNotFound", $"Could not find an inventory for item with itemId '{request.ItemId}'"));
            }

            return Result.Success(snapshot.Value);
        }
    }
}
