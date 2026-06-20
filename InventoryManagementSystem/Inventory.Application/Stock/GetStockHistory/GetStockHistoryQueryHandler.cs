using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Application.Abstractions;
using Inventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.GetStockHistory
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

            var mutations = await _mutationRepository.GetAllByItemIdAsync(request.ItemId, cancellationToken);

            return Result<IEnumerable<InventoryMutation>>.Success(mutations);
        }
    }
}
