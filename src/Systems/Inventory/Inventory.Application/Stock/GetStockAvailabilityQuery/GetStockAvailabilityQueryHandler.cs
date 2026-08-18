using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Common.Results.Problems;
using Inventory.Application.Abstractions;
using Inventory.Domain.Models;

namespace Inventory.Application.Stock.GetStockAvailabilityQuery
{
    public sealed class GetStockAvailabilityQueryHandler(
        IInventoryReservationCapacityRepository capacityRepository,
        IInventoryStockRepository stockRepository)
        : IRequestHandler<GetStockAvailabilityQuery, Result<InventoryStockAvailability>>
    {
        private readonly IInventoryReservationCapacityRepository _capacityRepository = capacityRepository;
        private readonly IInventoryStockRepository _stockRepository = stockRepository;

        public async Task<Result<InventoryStockAvailability>> HandleAsync(GetStockAvailabilityQuery request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var snapshot = await _stockRepository.GetByItemIdAsync(request.ItemId, cancellationToken);

            if (snapshot is null)
            {
                return Result.Failure<InventoryStockAvailability>(
                    ProblemFactory.NotFound("error: InventoryNotFound", $"Could not find an inventory for item with itemId '{request.ItemId}'"));
            }

            var stock = snapshot.Value;

            var capacitySnapshot = await _capacityRepository.GetByItemIdAsync(request.ItemId, cancellationToken);

            var reservedQuantity = capacitySnapshot?.Value.ReservedQuantity ?? Quantity.Create(0m, stock.Quantity.Unit);

            var availability = InventoryStockAvailability.Create(stock.Item, stock.Quantity, reservedQuantity);

            return availability;
        }
    }
}
