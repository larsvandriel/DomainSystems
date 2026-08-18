using Common.Messaging.Abstractions.Validation;

namespace Inventory.Application.Reservations.SearchReservation
{
    public sealed class SearchReservationQueryValidator : IRequestValidator<SearchReservationsQuery>
    {
        public ValueTask<ValidationResult> ValidateAsync(
            SearchReservationsQuery request,
            CancellationToken cancellationToken = default)
        {
            var result = new ValidationResult();

            if(request.Filter.ItemId is not null && request.Filter.ItemId == Guid.Empty)
                result.Add(nameof(request.Filter.ItemId), "The itemId should not be empty");

            if (request.Page.Number < 1)
                result.Add(nameof(request.Page.Number), "The page number must be at least 1.");

            if (request.Page.Size is < 1 or > 100)
                result.Add(nameof(request.Page.Size), "The page size must be between 1 and 100.");

            foreach (var duplicate in request.Sort.GroupBy(option => option.Field).Where(group => group.Count() > 1))
            {
                result.Add(nameof(request.Sort), $"Sort field '{duplicate.Key}' was specified more than once.");
            }

            if (!Enum.IsDefined(request.Filter.Selection))
                result.Add(nameof(request.Filter.Selection), $"The reservation selection is invalid.");

            foreach (var option in request.Sort)
            {
                if (!Enum.IsDefined(option.Field))
                    result.Add(nameof(request.Sort), $"Sort field '{option.Field}' is invalid.");

                if (!Enum.IsDefined(option.Direction))
                    result.Add(nameof(request.Sort), $"Sort direction '{option.Direction}' is invalid.");
            }

            return ValueTask.FromResult(result);
        }
    }
}
