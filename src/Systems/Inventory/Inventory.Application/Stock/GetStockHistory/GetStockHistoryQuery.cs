using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Domain.Models;

namespace Inventory.Application.Stock.GetStockHistory
{
    public sealed record GetStockHistoryQuery : IRequest<Result<IReadOnlyList<InventoryMutation>>>
    {
        public required Guid ItemId { get; init; }
    }
}
