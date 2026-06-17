using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Application.Stock.Abstractions;
using Inventory.Application.Stock.Commands;
using Inventory.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.Handlers
{
    public sealed class DecreaseStockCommandHandler(
        IUnitConverter unitConverter,
        IInventoryStockRepository stockRepository,
        IInventoryMutationRepository mutationRepository) : IRequestHandler<DecreaseStockCommand, Result>
    {
        private readonly IUnitConverter _unitConverter = unitConverter;
        private readonly IInventoryStockRepository _stockRepository = stockRepository;
        private readonly IInventoryMutationRepository _mutationRepository = mutationRepository;

        public async Task<Result> HandleAsync(DecreaseStockCommand request, CancellationToken cancellationToken = default)
        {
            var quantity = Quantity.Create(request.Amount, request.Unit);

            var stock = await _stockRepository.GetByItemIdAsync(request.ItemId, cancellationToken);

            if (stock is null)
            {
                stock = InventoryStock.Create(request.ItemId, quantity);
                await _stockRepository.AddAsync(stock, cancellationToken);
            }
            else
            {
                stock.Decrease(quantity, _unitConverter);
                await _stockRepository.UpdateAsync(stock, cancellationToken);
            }

            var mutation = InventoryMutation.CreateDecrease(request.ItemId, quantity);

            await _mutationRepository.AddAsync(mutation, cancellationToken);

            return Result.Success();
        }
    }
}
