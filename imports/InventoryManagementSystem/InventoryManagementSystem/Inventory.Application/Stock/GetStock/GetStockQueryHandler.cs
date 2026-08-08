using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Application.Abstractions;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
                return Result<InventoryStock>.Failure(
                    ProblemDetailsFactory.NotFound("error: InventoryNotFound", $"Could not find an inventory for item with itemId '{request.ItemId}'"));
            }

            return Result<InventoryStock>.Success(snapshot.Value);
        }
    }
}
