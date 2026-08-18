using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Domain.Models;

namespace Inventory.Application.Stock.GetCurrentStock
{
    public sealed record GetCurrentStockQuery : IRequest<Result<IReadOnlyList<InventoryStock>>>;
}
