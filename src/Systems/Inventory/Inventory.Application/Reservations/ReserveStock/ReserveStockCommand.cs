using Common.Messaging.Abstractions.Requests;
using Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Reservations.ReserveStock
{
    public sealed record ReserveStockCommand(
        Guid ItemId,
        decimal Amount,
        string Unit,
        string? Reference,
        DateTimeOffset? ExpiresAt) : IRequest<Result>;
}
