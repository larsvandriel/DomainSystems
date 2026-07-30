using Common.Results;
using Inventory.Application.Abstractions;
using Inventory.Application.Reservations.Enums;
using Inventory.Domain.Enums;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Reservations.Services
{
    public sealed class ReservationQueryService(IInventoryReservationRepository reservationRepository) : IReservationQueryService
    {
        private readonly IInventoryReservationRepository _reservationRepository = reservationRepository;

        public async Task<Result<IReadOnlyList<InventoryReservation>>> GetAsync(ReservationQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var errors = Validate(filter);

            if(errors.Any)
                return Result<IReadOnlyList<InventoryReservation>>.Failure(ProblemDetailsFactory.CreateValidation(
                    type: "error:ReservationFilterInvalid",
                    detail: "The set filter was invalid.",
                    errors: errors.ToDictionary()));

            var activeAt = DateTimeOffset.UtcNow;

            var reservations = await _reservationRepository.GetAsync(filter, activeAt, cancellationToken);

            return Result<IReadOnlyList<InventoryReservation>>.Success(reservations);
        }

        private static ValidationErrors Validate(ReservationQueryFilter filter)
        {
            var errors = new ValidationErrors();

            if (filter.ItemId == Guid.Empty)
            {
                errors.Add(nameof(filter.ItemId), "The itemId should not be a default value.");
            }

            return errors;
        }
    }
}
