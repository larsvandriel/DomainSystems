using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Application.Abstractions;
using Inventory.Domain.Models;

namespace Inventory.Application.Stock.GetStockHistory
{
    public sealed class GetStockHistoryQueryHandler(
        IInventoryMutationRepository mutationRepository)
        : IRequestHandler<GetStockHistoryQuery, Result<IReadOnlyList<InventoryMutation>>>
    {
        private readonly IInventoryMutationRepository _mutationRepository = mutationRepository;

        public async Task<Result<IReadOnlyList<InventoryMutation>>> HandleAsync(GetStockHistoryQuery request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var mutations = await _mutationRepository.GetAllByItemIdAsync(request.ItemId, cancellationToken);

            return Result.Success(mutations);
        }
    }
}
