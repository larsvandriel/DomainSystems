using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Results.Problems
{
    public enum ProblemKind
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Unauthorized = 4,
        Forbidden = 5,
        Unexpected = 6
    }
}
