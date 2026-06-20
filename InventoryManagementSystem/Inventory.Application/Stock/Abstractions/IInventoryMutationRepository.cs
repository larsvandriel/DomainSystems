using Common.Results;
using Inventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.Abstractions
{
    public interface IInventoryMutationRepository
    {
        Task AddAsync(InventoryMutation mutation, CancellationToken cancellationToken);
        Task<IReadOnlyList<InventoryMutation>> GetAllByItemIdAsync(Guid itemId, CancellationToken cancellationToken);
    }
}
