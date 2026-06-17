using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Application.Stock.Abstractions;
using Inventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.Queries
{
    public sealed class GetStockQueryHandler(IInventoryStockRepository stockRepository) : IRequestHandler<GetStockQuery, Result<InventoryStock>>
    {
        private readonly IInventoryStockRepository _stockRepository = stockRepository;
        
        public async Task<Result<InventoryStock>> HandleAsync(GetStockQuery request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var result = await _stockRepository.GetByItemIdAsync(request.ItemId, cancellationToken);

            if(result is null)
            {
                return Result<InventoryStock>.Failure("Stock not found.");
            }

            return result;
        }
    }
}
