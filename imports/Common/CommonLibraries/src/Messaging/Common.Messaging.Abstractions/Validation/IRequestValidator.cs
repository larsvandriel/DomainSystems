using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Messaging.Abstractions.Validation
{
    public interface IRequestValidator<in TRequest>
    {
        ValueTask<IReadOnlyCollection<ValidationFailure>> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
