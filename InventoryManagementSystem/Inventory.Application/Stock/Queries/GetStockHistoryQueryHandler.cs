using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Application.Stock.Abstractions;
using Inventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.Queries
{
    public sealed class GetStockHistoryQueryHandler(
        IInventoryStockRepository stockRepository,
        IInventoryMutationRepository mutationRepository) : IRequestHandler<GetStockHistoryQuery, Result<IEnumerable<InventoryMutation>>>
    {
        private readonly IInventoryStockRepository _stockRepository = stockRepository;
        private readonly IInventoryMutationRepository _mutationRepository = mutationRepository;

        public async Task<Result<IEnumerable<InventoryMutation>>> HandleAsync(GetStockHistoryQuery request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var stock = await _stockRepository.GetByItemIdAsync(request.ItemId, cancellationToken);

            if (stock is null)
            {
                return Result<IEnumerable<InventoryMutation>>.Failure("Stock not found.");
            }

            var result = _mutationRepository.GetAllByItemIdAsync(request.ItemId, cancellationToken);

            return result;
        }
    }
}
