using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.Queries
{
    public sealed record GetStockQuery : IRequest<Result<InventoryStock>>
    {
        public required Guid ItemId { get; init; }
    }
}
