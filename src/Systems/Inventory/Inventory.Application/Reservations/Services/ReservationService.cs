using Common.Optional;
using Common.Persistence.Concurrency;
using Common.Results;
using Inventory.Application.Abstractions;
using Inventory.Application.Stock.Services;
using Inventory.Domain.Enums;
using Inventory.Domain.Models;

namespace Inventory.Application.Reservations.Services
{
    public sealed class ReservationService(
        IInventoryReservationRepository reservationRepository,
        IInventoryReservationCapacityRepository capacityRepository,
        IStockMutationService stockMutationService,
        IInventoryItemRepository itemRepository,
        IInventoryStockRepository stockRepository,
        QuantityNormalizer quantityNormalizer,
        QuantityCalculator quantityCalculator) : IReservationService
    {
        private readonly IInventoryReservationRepository _reservationRepository = reservationRepository;
        private readonly IInventoryReservationCapacityRepository _capacityRepository = capacityRepository;
        private readonly IStockMutationService _stockMutationService = stockMutationService;
        private readonly IInventoryItemRepository _itemRepository = itemRepository;
        private readonly IInventoryStockRepository _stockRepository = stockRepository;
        private readonly QuantityNormalizer _quantityNormalizer = quantityNormalizer;
        private readonly QuantityCalculator _quantityCalculator = quantityCalculator;

        public async Task<Result> AdjustReservationAsync(Guid reservationId, Quantity? requestedQuantity, string? reference, Optional<DateTimeOffset?> expiresAt, CancellationToken cancellationToken)
        {
            var reservationSnapshot = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);

            if (reservationSnapshot is null)
                return Result.Failure(ProblemDetailsFactory.NotFound(
                    type: "error:ReservationNotFound",
                    detail: "Tried to update reservation, but the reservation was not found."));

            var reservation = reservationSnapshot.Value;

            if (reservation.Status != ReservationStatus.Open)
                return Result.Failure(ProblemDetailsFactory.BusinessRule(
                    "error:AdjustmentClosedReservation",
                    $"The reservation with reference {reservation.Reference} was already closed."));

            if (reservation.ExpiresAt <= DateTimeOffset.UtcNow)
                return Result.Failure(ProblemDetailsFactory.BusinessRule(
                    "error:AdjustExpiredReservation",
                    $"The reservation with reference {reservation.Reference} has already expired."));

            if (requestedQuantity is not null)
            {
                var stockSnapshot = await _stockRepository.GetByItemIdAsync(reservation.Item.Id, cancellationToken);

                if (stockSnapshot is null)
                    return Result.Failure(ProblemDetailsFactory.BusinessRule(
                        type: "error:InventoryStockNotFound",
                        detail: $"Stock for item '{reservation.Item.Id}' was not found."));

                var capacitySnapshot = await _capacityRepository.GetByItemIdAsync(reservation.Item.Id, cancellationToken);

                if (capacitySnapshot is null)
                    return Result.Failure(ProblemDetailsFactory.Unexpected(
                        exception: new InvalidOperationException("Reservation capacity is missing."),
                        detail: "The reservation capacity is inconsistent."));

                var oldQuantityResult = _quantityNormalizer.NormalizeTo(reservation.Quantity, capacitySnapshot.Value.ReservedQuantity.Unit);

                if (oldQuantityResult.IsFailure)
                    return oldQuantityResult;

                var newQuantityResult = _quantityNormalizer.NormalizeTo(requestedQuantity, capacitySnapshot.Value.ReservedQuantity.Unit);

                if (newQuantityResult.IsFailure)
                    return newQuantityResult;

                var withoutCurrentResult = _quantityCalculator.Subtract(capacitySnapshot.Value.ReservedQuantity, oldQuantityResult.Value);

                if (withoutCurrentResult.IsFailure)
                    return withoutCurrentResult;

                var proposedReservedResult = _quantityCalculator.Add(withoutCurrentResult.Value, newQuantityResult.Value);

                if(proposedReservedResult.IsFailure)
                    return proposedReservedResult;

                if (!stockSnapshot.Value.Quantity.IsGreaterThanOrSame(proposedReservedResult.Value))
                    return Result.Failure(ProblemDetailsFactory.BusinessRule(
                        type: "error:InsufficientStock",
                        "The requested quantity exceeds available stock."));

                capacitySnapshot.Value.Adjust(oldQuantityResult.Value, newQuantityResult.Value);

                reservation.AdjustAmount(requestedQuantity);

                await _capacityRepository.UpdateAsync(capacitySnapshot.Value, capacitySnapshot.Token, cancellationToken);
            }

            if (reference is not null && !string.Equals(reference, reservation.Reference, StringComparison.Ordinal))
            {
                var foundReservation = await _reservationRepository.GetByReference(reference, cancellationToken);
                if (foundReservation is not null && foundReservation.Id != reservation.Id)
                    return Result.Failure(ProblemDetailsFactory.Conflict(
                        type: "error:ReservationReferenceAlreadyExists",
                        detail: $"There is already an reservation with reference '{reference}'."));

                reservation.AdjustReference(reference);
            }

            if (expiresAt.IsSpecified)
            {
                reservation.AdjustExpiresAt(expiresAt.Value);
            }

            await _reservationRepository.UpdateAsync(reservation, reservationSnapshot.Token, cancellationToken);

            return Result.Success();
        }

        public async Task<Result> CancelReservationAsync(Guid reservationId, CancellationToken cancellationToken)
        {
            var reservationSnapshot = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);

            if (reservationSnapshot == null)
                return Result.Failure(ProblemDetailsFactory.NotFound(
                    "error:ReservationNotFound",
                    $"The reservation with id {reservationId} was not found."));

            var reservation = reservationSnapshot.Value;

            if (reservation.Status != ReservationStatus.Open)
                return Result.Failure(ProblemDetailsFactory.BusinessRule(
                    "error:AdjustmentClosedReservation",
                    $"The reservation with reference {reservation.Reference} was already closed."));

            if (reservation.ExpiresAt <= DateTimeOffset.UtcNow)
                return Result.Failure(ProblemDetailsFactory.BusinessRule(
                    "error:CancelExpiredReservation",
                    $"The reservation with reference {reservation.Reference} has already expired."));

            var capacitySnapshot = await _capacityRepository.GetByItemIdAsync(reservation.Item.Id, cancellationToken);

            if (capacitySnapshot is null)
                return Result.Failure(ProblemDetailsFactory.Unexpected(
                        exception: new InvalidOperationException("Reservation capacity is missing."),
                        detail: "The reservation capacity is inconsistent."));

            var normalizedResult = _quantityNormalizer.NormalizeTo(reservation.Quantity, capacitySnapshot.Value.ReservedQuantity.Unit);

            if (normalizedResult.IsFailure)
                return normalizedResult;

            capacitySnapshot.Value.Release(normalizedResult.Value);
            reservation.Cancel();

            await _capacityRepository.UpdateAsync(capacitySnapshot.Value, capacitySnapshot.Token, cancellationToken);

            await _reservationRepository.UpdateAsync(reservation, reservationSnapshot.Token, cancellationToken);

            return Result.Success();
        }

        public async Task<Result> CommitReservationAsync(Guid reservationId, CancellationToken cancellationToken)
        {
            var reservationSnapshot = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);

            if (reservationSnapshot == null)
                return Result.Failure(ProblemDetailsFactory.NotFound(
                    "error:ReservationNotFound",
                    $"The reservation with id {reservationId} was not found."));

            var reservation = reservationSnapshot.Value;

            if (reservation.Status != ReservationStatus.Open)
                return Result.Failure(ProblemDetailsFactory.BusinessRule(
                    "error:AdjustmentClosedReservation",
                    $"The reservation with reference {reservation.Reference} was already closed."));

            if (reservation.ExpiresAt <= DateTimeOffset.UtcNow)
                return Result.Failure(ProblemDetailsFactory.BusinessRule(
                    "error:CommitExpiredReservation",
                    $"The reservation with reference {reservation.Reference} has already expired."));

            var capacitySnapshot = await _capacityRepository.GetByItemIdAsync(reservation.Item.Id, cancellationToken);

            if (capacitySnapshot is null)
                return Result.Failure(ProblemDetailsFactory.Unexpected(
                        exception: new InvalidOperationException("Reservation capacity is missing."),
                        detail: "The reservation capacity is inconsistent."));

            var normalizedResult = _quantityNormalizer.NormalizeTo(reservation.Quantity, capacitySnapshot.Value.ReservedQuantity.Unit);

            if (normalizedResult.IsFailure)
                return normalizedResult;

            var stockResult = await _stockMutationService.DecreaseAsync(
                reservation.Item.Id,
                reservation.Item.Name,
                reservation.Quantity.Value,
                reservation.Quantity.Unit,
                cancellationToken);

            if (stockResult.IsFailure)
                return stockResult;

            capacitySnapshot.Value.Release(normalizedResult.Value);
            reservation.Commit();

            await _capacityRepository.UpdateAsync(capacitySnapshot.Value, capacitySnapshot.Token, cancellationToken);

            await _reservationRepository.UpdateAsync(reservation, reservationSnapshot.Token, cancellationToken);

            return Result.Success();
        }

        public async Task<Result> CreateReservationAsync(Guid itemId, Quantity requestedQuantity, string? reference, DateTimeOffset? expiresAt, CancellationToken cancellationToken)
        {
            var inventoryItem = await _itemRepository.GetByIdAsync(itemId, cancellationToken);

            if (inventoryItem is null)
                return Result.Failure(ProblemDetailsFactory.BusinessRule(
                    type: "error:CreateReservationForNonExistingStock",
                    detail: "Tried to create reservation while stock was not found."));

            var stockSnapshot = await _stockRepository.GetByItemIdAsync(itemId, cancellationToken);

            if (stockSnapshot is null)
                return Result.Failure(ProblemDetailsFactory.BusinessRule(
                    type: "error:InventoryStockNotFound",
                    $"Stock for item '{itemId}' was not found."));

            var stock = stockSnapshot.Value;

            var (capacity, capacityToken) = await GetOrCreateCapacityAsync(inventoryItem, stock.Quantity.Unit, cancellationToken);

            var normalizedResult = _quantityNormalizer.NormalizeTo(requestedQuantity, capacity.ReservedQuantity.Unit);

            if (normalizedResult.IsFailure)
                return normalizedResult;

            var newReservedResult = _quantityCalculator.Add(capacity.ReservedQuantity, normalizedResult.Value);

            if (newReservedResult.IsFailure)
                return newReservedResult;

            if (!stock.Quantity.IsGreaterThanOrSame(newReservedResult.Value))
                return Result.Failure(ProblemDetailsFactory.BusinessRule(
                    type: "error:InsufficientStock",
                    detail: $"The requested quantity exceeds available stock."));

            if (reference is not null)
            {
                var existingReservation = await _reservationRepository.GetByReference(reference, cancellationToken);

                if (existingReservation != null)
                    return Result.Failure(ProblemDetailsFactory.Conflict(
                        type: "error:ReservationReferenceAlreadyExists",
                        detail: $"There is already an reservation with reference '{reference}'."));
            }

            capacity.Reserve(normalizedResult.Value);

            var reservation = InventoryReservation.Create(inventoryItem, requestedQuantity, reference, expiresAt);

            await _reservationRepository.AddAsync(reservation, cancellationToken);

            await SaveCapacityAsync(capacity, capacityToken, cancellationToken);

            return Result.Success();
        }

        public async Task<Result> ExpireReservationAsync(Guid reservationId, DateTimeOffset utcNow, CancellationToken cancellationToken)
        {
            var reservationSnapshot = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);

            if (reservationSnapshot == null)
                return Result.Failure(ProblemDetailsFactory.NotFound(
                    "error:ReservationNotFound",
                    $"The reservation with id {reservationId} was not found."));

            var reservation = reservationSnapshot.Value;

            if (reservation.Status != ReservationStatus.Open)
                return Result.Failure(ProblemDetailsFactory.BusinessRule(
                    "error:AdjustmentClosedReservation",
                    $"The reservation with reference {reservation.Reference} was already closed."));

            if (reservation.ExpiresAt is null)
            {
                return Result.Failure(
                    ProblemDetailsFactory.BusinessRule(
                        type: "error:ReservationHasNoExpiration",
                        detail: "A reservation without an expiration cannot expire."));
            }

            if (reservation.ExpiresAt > utcNow)
                return Result.Failure(ProblemDetailsFactory.BusinessRule(
                    type: "error:ExpireReservationBeforeExpireTime",
                    detail: $"The reservation expires at {reservation.ExpiresAt}."));

            var capacitySnapshot = await _capacityRepository.GetByItemIdAsync(reservation.Item.Id, cancellationToken);

            if (capacitySnapshot is null)
                return Result.Failure(ProblemDetailsFactory.Unexpected(
                        exception: new InvalidOperationException("Reservation capacity is missing."),
                        detail: "The reservation capacity is inconsistent."));

            var normalizedResult = _quantityNormalizer.NormalizeTo(reservation.Quantity, capacitySnapshot.Value.ReservedQuantity.Unit);

            if (normalizedResult.IsFailure)
                return normalizedResult;

            capacitySnapshot.Value.Release(normalizedResult.Value);
            reservation.Expire(utcNow);

            await _capacityRepository.UpdateAsync(capacitySnapshot.Value, capacitySnapshot.Token, cancellationToken);

            await _reservationRepository.UpdateAsync(reservation, reservationSnapshot.Token, cancellationToken);

            return Result.Success();
        }

        public async Task<Result> ReleaseReservationAsync(Guid reservationId, CancellationToken cancellationToken)
        {
            var reservationSnapshot = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);

            if (reservationSnapshot == null)
                return Result.Failure(ProblemDetailsFactory.NotFound(
                    "error:ReservationNotFound",
                    $"The reservation with id {reservationId} was not found."));

            var reservation = reservationSnapshot.Value;

            if (reservation.Status != ReservationStatus.Open)
                return Result.Failure(ProblemDetailsFactory.BusinessRule(
                    "error:AdjustmentClosedReservation",
                    $"The reservation with reference {reservation.Reference} was already closed."));

            if (reservation.ExpiresAt <= DateTimeOffset.UtcNow)
                return Result.Failure(ProblemDetailsFactory.BusinessRule(
                    "error:ReleaseExpiredReservation",
                    $"The reservation with reference {reservation.Reference} has already expired."));

            var capacitySnapshot = await _capacityRepository.GetByItemIdAsync(reservation.Item.Id, cancellationToken);

            if (capacitySnapshot is null)
                return Result.Failure(ProblemDetailsFactory.Unexpected(
                        exception: new InvalidOperationException("Reservation capacity is missing."),
                        detail: "The reservation capacity is inconsistent."));

            var normalizedResult = _quantityNormalizer.NormalizeTo(reservation.Quantity, capacitySnapshot.Value.ReservedQuantity.Unit);

            if (normalizedResult.IsFailure)
                return normalizedResult;

            capacitySnapshot.Value.Release(normalizedResult.Value);
            reservation.Release();

            await _capacityRepository.UpdateAsync(capacitySnapshot.Value, capacitySnapshot.Token, cancellationToken);

            await _reservationRepository.UpdateAsync(reservation, reservationSnapshot.Token, cancellationToken);

            return Result.Success();
        }

        private async Task<(InventoryReservationCapacity, ConcurrencyToken? Token)> GetOrCreateCapacityAsync(
            InventoryItem item,
            string stockUnit,
            CancellationToken cancellationToken)
        {
            var snapshot = await _capacityRepository.GetByItemIdAsync(item.Id, cancellationToken);

            if (snapshot is not null)
                return (snapshot.Value, snapshot.Token);

            return (InventoryReservationCapacity.Create(item, stockUnit), null);
        }

        private Task SaveCapacityAsync(InventoryReservationCapacity capacity, ConcurrencyToken? concurrencyToken, CancellationToken cancellationToken)
        {
            return concurrencyToken is null
                ? _capacityRepository.AddAsync(capacity, cancellationToken)
                : _capacityRepository.UpdateAsync(capacity, concurrencyToken, cancellationToken);
        }
    }
}
