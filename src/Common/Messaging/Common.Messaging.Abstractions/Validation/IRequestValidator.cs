using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Messaging.Abstractions.Validation
{
    public interface IRequestValidator<in TRequest>
    {
        ValueTask<ValidationResult> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
