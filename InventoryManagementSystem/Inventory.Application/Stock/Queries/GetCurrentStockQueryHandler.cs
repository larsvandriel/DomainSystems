using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Application.Stock.Abstractions;
using Inventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.Queries
{
    public sealed class GetCurrentStockQueryHandler(IInventoryStockRepository stockRepository) : IRequestHandler<GetCurrentStockQuery, Result<IEnumerable<InventoryStock>>>
    {
        private readonly IInventoryStockRepository _stockRepository = stockRepository;

        public async Task<Result<IEnumerable<InventoryStock>>> HandleAsync(GetCurrentStockQuery request, CancellationToken cancellationToken = default)
        {
            var result = await _stockRepository.GetAllAsync(cancellationToken);
            return Result<IEnumerable<InventoryStock>>.Success(result);
        }
    }
}
