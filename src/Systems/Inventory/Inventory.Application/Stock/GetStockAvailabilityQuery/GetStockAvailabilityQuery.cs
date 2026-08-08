using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.GetStockAvailabilityQuery
{
    public sealed record GetStockAvailabilityQuery(Guid ItemId) : IRequest<Result<InventoryStockAvailability>>;
}
