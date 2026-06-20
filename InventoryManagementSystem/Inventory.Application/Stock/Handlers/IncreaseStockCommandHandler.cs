using Common.Messaging.Abstractions.Requests;
using Common.Persistence.Abstractions;
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
        ITransactionalExecutor transactionalExecutor,
        IUnitConverter unitConverter,
        IInventoryStockRepository stockRepository,
        IInventoryItemRepository itemRepository,
        IInventoryMutationRepository mutationRepository) : IRequestHandler<IncreaseStockCommand, Result>
    {
        private readonly ITransactionalExecutor _transactionalExecutor = transactionalExecutor;
        private readonly IUnitConverter _unitConverter = unitConverter;
        private readonly IInventoryStockRepository _stockRepository = stockRepository;
        private readonly IInventoryItemRepository _itemRepository = itemRepository;
        private readonly IInventoryMutationRepository _mutationRepository = mutationRepository;

        public Task<Result> HandleAsync(IncreaseStockCommand request, CancellationToken cancellationToken = default)
        {
            return _transactionalExecutor.ExecuteAsync(async ct =>
            {
                var item = await _itemRepository.GetByIdAsync(request.ItemId, ct);

                if (item is null)
                {
                    item = InventoryItem.Create(request.ItemId, request.ItemName);
                    await _itemRepository.AddAsync(item, ct);
                }

                var quantity = Quantity.Create(request.Amount, request.Unit);

                var stock = await _stockRepository.GetByItemIdAsync(request.ItemId, ct);

                if (stock is null)
                {
                    stock = InventoryStock.Create(item, Quantity.Create(0, request.Unit));
                    stock.Increase(quantity, _unitConverter);

                    await _stockRepository.AddAsync(stock, ct);
                }
                else
                {
                    stock.Increase(quantity, _unitConverter);
                    await _stockRepository.UpdateAsync(stock, ct);
                }

                foreach (var mutation in stock.DequeueMutations())
                {
                    await _mutationRepository.AddAsync(mutation, ct);
                }

                return Result.Success();
            }, cancellationToken);
        }
    }
}
