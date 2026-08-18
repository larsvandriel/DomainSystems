using Common.Messaging.Abstractions.Requests;
using Common.Results;

namespace Inventory.Application.Stock.ApplyStockCount
{
    public sealed record ApplyStockCountCommand(IReadOnlyCollection<StockCountLine> Lines) : IRequest<Result>;
}
