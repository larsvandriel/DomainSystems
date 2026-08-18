using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Application.Abstractions;
using Inventory.Domain.Models;

namespace Inventory.Application.Stock.GetCurrentStockAvailabilityQuery
{
    public sealed class GetCurrentStockAvailabilityQueryHandler(
        IInventoryReservationCapacityRepository capacityRepository,
        IInventoryStockRepository stockRepository)
        : IRequestHandler<GetCurrentStockAvailabilityQuery, Result<IReadOnlyList<InventoryStockAvailability>>>
    {
        private readonly IInventoryReservationCapacityRepository _capacityRepository = capacityRepository;
        private readonly IInventoryStockRepository _stockRepository = stockRepository;

        public async Task<Result<IReadOnlyList<InventoryStockAvailability>>> HandleAsync(GetCurrentStockAvailabilityQuery request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var stock = await _stockRepository.GetAllAsync(cancellationToken);

            var result = new List<InventoryStockAvailability>();

            var capacities = await _capacityRepository.GetAllAsync(cancellationToken);

            var byItemId = capacities.ToDictionary(x => x.Item.Id);

            foreach(var stockItem in stock)
            {
                var reserved = byItemId.TryGetValue(stockItem.Item.Id, out var capacity) ? capacity.ReservedQuantity : Quantity.Create(0m, stockItem.Quantity.Unit);

                var inventoryStockAvailabilty = InventoryStockAvailability.Create(stockItem.Item, stockItem.Quantity, reserved);

                result.Add(inventoryStockAvailabilty);
            }

            return result;
        }
    }
}
