using System.Globalization;

namespace Common.Results.Problems
{
    public static class ProblemFactory
    {
        public static Problem Validation(string code, string detail, IReadOnlyDictionary<string, string[]> errors)
        {
            return new ValidationProblem
            {
                Code = code,
                Title = "Validation error",
                Detail = detail,
                Kind = ProblemKind.Validation,
                Errors = errors
            };
        }

        public static Problem BusinessRule(string code, string detail)
        {
            return new Problem
            {
                Code = code,
                Title = "Business rule violation",
                Detail = detail,
                Kind = ProblemKind.Conflict
            };
        }

        public static Problem NotFound(string code, string detail)
        {
            return new Problem
            {
                Code = code,
                Title = "Not found",
                Detail = detail,
                Kind = ProblemKind.NotFound
            };
        }

        public static Problem Conflict(string code, string detail)
        {
            return new Problem
            {
                Code = code,
                Title = "Conflict",
                Detail = detail,
                Kind = ProblemKind.Conflict
            };
        }

        public static Problem Forbidden(string code, string detail)
        {
            return new Problem
            {
                Code = code,
                Title = "Forbidden",
                Detail = detail,
                Kind = ProblemKind.Forbidden
            };
        }

        public static Problem Unexpected()
        {
            return new Problem
            {
                Code = "unexpected_error",
                Title = "Unexpected error",
                Detail = "An unexpected error occurred.",
                Kind = ProblemKind.Unexpected
            };
        }
    }
}
