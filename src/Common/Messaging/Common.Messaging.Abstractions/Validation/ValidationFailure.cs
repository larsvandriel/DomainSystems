using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Messaging.Abstractions.Validation
{
    public sealed record ValidationFailure(string PropertyName, string ErrorMessage, string? ErrorCode = null);
}
