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
    public sealed class ApplyStockCountCommandHandler(
        ITransactionalExecutor transactionalExecutor,
        IUnitConverter unitConverter,
        IInventoryItemRepository itemRepository,
        IInventoryStockRepository stockRepository,
        IInventoryMutationRepository mutationRepository)
        : IRequestHandler<ApplyStockCountCommand, Result>
    {
        public Task<Result> HandleAsync(ApplyStockCountCommand request, CancellationToken cancellationToken = default)
        {
            return transactionalExecutor.ExecuteAsync(async ct =>
            {
                if (request.Lines is null || request.Lines.Count == 0)
                    return Result.Failure("No stock count lines provided.");

                foreach (var line in request.Lines)
                {
                    var item = await itemRepository.GetByIdAsync(line.ItemId, ct);

                    if (item is null)
                    {
                        item = InventoryItem.Create(line.ItemId, line.ItemName);
                        await itemRepository.AddAsync(item, ct);
                    }

                    var countedQuantity = Quantity.Create(line.CountedAmount, line.Unit);

                    var stock = await stockRepository.GetByItemIdAsync(line.ItemId, ct);

                    if (stock is null)
                    {
                        stock = InventoryStock.Create(item, Quantity.Create(0, line.Unit));

                        stock.Adjust(countedQuantity);

                        await stockRepository.AddAsync(stock, ct);
                    }
                    else
                    {
                        var targetQuantity = stock.Quantity.Unit == countedQuantity.Unit
                            ? countedQuantity
                            : unitConverter.Convert(countedQuantity, stock.Quantity.Unit);

                        stock.Adjust(targetQuantity);

                        await stockRepository.UpdateAsync(stock, ct);
                    }

                    foreach (var mutation in stock.DequeueMutations())
                    {
                        await mutationRepository.AddAsync(mutation, ct);
                    }
                }

                return Result.Success();
            }, cancellationToken);
        }
    }
}
