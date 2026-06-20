using Inventory.Application.Abstractions;
using Inventory.Application.Stock.ApplyStockCount;
using Inventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Services
{
    public sealed class StockAdjustmentService(
        IUnitConverter unitConverter,
        IInventoryItemRepository itemRepository,
        IInventoryStockRepository stockRepository,
        IInventoryMutationRepository mutationRepository)
        : IStockAdjustmentService
    {
        public async Task ApplyAsync(StockCountLine line, CancellationToken cancellationToken)
        {
            var item = await itemRepository.GetByIdAsync(line.ItemId, cancellationToken);

            if (item is null)
            {
                item = InventoryItem.Create(line.ItemId, line.ItemName);
                await itemRepository.AddAsync(item, cancellationToken);
            }

            var countedQuantity = Quantity.Create(line.CountedAmount, line.Unit);

            var stock = await stockRepository.GetByItemIdAsync(line.ItemId, cancellationToken);

            if (stock is null)
            {
                stock = InventoryStock.Create(item, Quantity.Create(0, line.Unit));
                stock.Adjust(countedQuantity);

                await stockRepository.AddAsync(stock, cancellationToken);
            }
            else
            {
                var targetQuantity = stock.Quantity.Unit == countedQuantity.Unit
                    ? countedQuantity
                    : unitConverter.Convert(countedQuantity, stock.Quantity.Unit);

                stock.Adjust(targetQuantity);

                await stockRepository.UpdateAsync(stock, cancellationToken);
            }

            foreach (var mutation in stock.DequeueMutations())
            {
                await mutationRepository.AddAsync(mutation, cancellationToken);
            }
        }
    }
}
