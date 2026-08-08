using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.GetCurrentStockAvailabilityQuery
{
    public sealed record GetCurrentStockAvailabilityQuery : IRequest<Result<IReadOnlyList<InventoryStockAvailability>>>;
}
