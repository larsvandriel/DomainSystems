using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Application.Abstractions;
using Inventory.Domain.Models;

namespace Inventory.Application.Stock.GetCurrentStock
{
    public sealed class GetCurrentStockQueryHandler(IInventoryStockRepository stockRepository) : IRequestHandler<GetCurrentStockQuery, Result<IReadOnlyList<InventoryStock>>>
    {
        private readonly IInventoryStockRepository _stockRepository = stockRepository;

        public async Task<Result<IReadOnlyList<InventoryStock>>> HandleAsync(GetCurrentStockQuery request, CancellationToken cancellationToken = default)
        {
            var result = await _stockRepository.GetAllAsync(cancellationToken);
            return Result.Success(result);
        }
    }
}
