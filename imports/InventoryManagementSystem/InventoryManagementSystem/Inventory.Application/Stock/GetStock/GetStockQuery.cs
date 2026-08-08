using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.GetStock
{
    public sealed record GetStockQuery : IRequest<Result<InventoryStock>>
    {
        public required Guid ItemId { get; init; }
    }
}
