using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Application.Abstractions;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.GetCurrentStock
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
