using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Application.Stock.Abstractions;
using Inventory.Application.Stock.Commands;
using Inventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.Handlers
{
    public sealed class IncreaseStockCommandHandler(
        IUnitConverter unitConverter,
        IInventoryStockRepository stockRepository,
        IInventoryItemRepository itemRepository,
        IInventoryMutationRepository mutationRepository) : IRequestHandler<IncreaseStockCommand, Result>
    {
        private readonly IUnitConverter _unitConverter = unitConverter;
        private readonly IInventoryStockRepository _stockRepository = stockRepository;
        private readonly IInventoryItemRepository _itemRepository = itemRepository;
        private readonly IInventoryMutationRepository _mutationRepository = mutationRepository;

        public async Task<Result> HandleAsync(IncreaseStockCommand request, CancellationToken cancellationToken = default)
        {
            var item = await _itemRepository.GetByIdAsync(request.ItemId, cancellationToken);

            if (item is null)
            {
                item = InventoryItem.Create(request.ItemId, request.ItemName);
                await _itemRepository.AddAsync(item, cancellationToken);
            }

            var quantity = Quantity.Create(request.Amount, request.Unit);

            var stock = await _stockRepository.GetByItemIdAsync(request.ItemId, cancellationToken);

            if (stock is null)
            {
                stock = InventoryStock.Create(item, Quantity.Create(0, request.Unit));
                stock.Increase(quantity, _unitConverter);

                await _stockRepository.AddAsync(stock, cancellationToken);
            }
            else
            {
                stock.Increase(quantity, _unitConverter);
                await _stockRepository.UpdateAsync(stock, cancellationToken);
            }

            foreach (var mutation in stock.DequeueMutations())
            {
                await _mutationRepository.AddAsync(mutation, cancellationToken);
            }

            return Result.Success();
        }
    }
}
