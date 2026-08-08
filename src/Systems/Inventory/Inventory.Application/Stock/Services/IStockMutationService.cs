using Common.Results;
using Inventory.Application.Stock.ApplyStockCount;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.Services
{
    public interface IStockMutationService
    {
        Task<Result> IncreaseAsync(Guid itemId, string itemName, decimal amount, string unit, CancellationToken cancellationToken = default);
        Task<Result> DecreaseAsync(Guid itemId, string itemName, decimal amount, string unit, CancellationToken cancellationToken = default);
        Task<Result> AdjustAsync(StockCountLine line, CancellationToken cancellationToken = default);
    }
}
