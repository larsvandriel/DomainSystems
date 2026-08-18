using Common.Persistence.Concurrency;
using Common.Results;
using Common.Results.Problems;
using Inventory.Application.Abstractions;
using Inventory.Application.Stock.ApplyStockCount;
using Inventory.Domain.Models;

namespace Inventory.Application.Stock.Services
{
    public sealed class StockMutationService(
        IInventoryItemRepository itemRepository,
        IInventoryStockRepository stockRepository,
        IInventoryMutationRepository mutationRepository,
        QuantityCalculator quantityCalculator,
        QuantityNormalizer quantityNormalizer) : IStockMutationService
    {
        private readonly IInventoryItemRepository _itemRepository = itemRepository;
        private readonly IInventoryStockRepository _stockRepository = stockRepository;
        private readonly IInventoryMutationRepository _mutationRepository = mutationRepository;
        private readonly QuantityCalculator _quantityCalculator = quantityCalculator;
        private readonly QuantityNormalizer _quantityNormalizer = quantityNormalizer;

        public async Task<Result> IncreaseAsync(Guid itemId, string itemName, decimal amount, string unit, CancellationToken cancellationToken = default)
        {
            var (stock, concurrencyToken) = await GetOrCreateStockAsync(itemId, itemName, unit, cancellationToken);

            var quantity = Quantity.Create(amount, unit);

            var result = _quantityCalculator.Add(stock.Quantity, quantity);
            if (result.IsFailure)
                return result;

            stock.ApplyIncrease(result.Value);

            return await SaveStockAndMutationsAsync(stock, concurrencyToken, cancellationToken);
        }

        public async Task<Result> DecreaseAsync(Guid itemId, string itemName, decimal amount, string unit, CancellationToken cancellationToken = default)
        {
            var (stock, concurrencyToken) = await GetOrCreateStockAsync(itemId, itemName, unit, cancellationToken);

            var quantity = Quantity.Create(amount, unit);

            var result = _quantityCalculator.Subtract(stock.Quantity, quantity);
            if (result.IsFailure)
                return result;

            stock.ApplyDecrease(result.Value);
            return await SaveStockAndMutationsAsync(stock, concurrencyToken, cancellationToken);
        }

        public async Task<Result> AdjustAsync(StockCountLine line, CancellationToken cancellationToken = default)
        {
            if(line.CountedAmount < 0)
                return Result.Failure(ProblemFactory.BusinessRule("error:AdjustStockNegativeAmount", "The stock adjustment must set the amount to a positive or zero value."));

            var (stock, concurrencyToken) = await GetOrCreateStockAsync(line.ItemId, line.ItemName, line.Unit, cancellationToken);

            var countedQuantity = Quantity.Create(line.CountedAmount, line.Unit);

            var result = _quantityNormalizer.NormalizeTo(countedQuantity, stock.Quantity.Unit);
            if (result.IsFailure)
                return result;

            stock.Adjust(result.Value);

            return await SaveStockAndMutationsAsync(stock, concurrencyToken, cancellationToken);
        }

        private async Task<(InventoryStock, ConcurrencyToken? token)> GetOrCreateStockAsync(Guid itemId, string itemName, string unit, CancellationToken cancellationToken)
        {
            var item = await GetOrCreateInventoryItemAsync(itemId, itemName, cancellationToken);

            var snapshot = await _stockRepository.GetByItemIdAsync(itemId, cancellationToken);

            if (snapshot is null)
                return (InventoryStock.Create(item, Quantity.Create(0, unit)), null);

            return (snapshot.Value, snapshot.Token);
        }

        private async Task<InventoryItem> GetOrCreateInventoryItemAsync(Guid itemId, string itemName, CancellationToken cancellationToken)
        {
            var item = await _itemRepository.GetByIdAsync(itemId, cancellationToken);
            if (item is null)
            {
                item = InventoryItem.Create(itemId, itemName);
                await _itemRepository.AddAsync(item, cancellationToken);
            }
            return item;
        }

        private async Task<Result> SaveStockAndMutationsAsync(InventoryStock stock, ConcurrencyToken? concurrencyToken, CancellationToken cancellationToken)
        {
            if (concurrencyToken is null)
                await _stockRepository.AddAsync(stock, cancellationToken);
            else
                await _stockRepository.UpdateAsync(stock, concurrencyToken, cancellationToken);

            foreach (var mutation in stock.PendingMutations)
            {
                await _mutationRepository.AddAsync(mutation, cancellationToken);
            }

            stock.ClearMutations();

            return Result.Success();
        }
    }
}
