using System.Diagnostics;
using Common.Results.Problems;
using Microsoft.AspNetCore.Http;
using AspNetResults = Microsoft.AspNetCore.Http.Results;

namespace Common.Results.AspNetCore.Mapping
{
    public sealed class HttpResultMapper(IHttpContextAccessor httpContextAccessor, TimeProvider timeProvider) : IHttpResultMapper
    {
        public IResult Map(Result result)
        {
            if (result.IsSuccess)
                return AspNetResults.NoContent();

            return MapProblem(result.Problem!);
        }

        public IResult Map<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return AspNetResults.Ok(result.Value);

            return MapProblem(result.Problem!);
        }

        private IResult MapProblem(Problem problem)
        {
            var httpContext = httpContextAccessor.HttpContext;

            var extensions = new Dictionary<string, object?>(problem.Extensions)
            {
                ["errorCode"] = problem.Code,
                ["timestamp"] = timeProvider.GetUtcNow()
            };

            var traceId = Activity.Current?.TraceId.ToString();

            if(traceId is not null)
                extensions["traceId"] = traceId;

            if(httpContext?.TraceIdentifier is { } correlationId)
                extensions["correlationId"] = correlationId;

            if(problem is ValidationProblem validationProblem)
                extensions["errors"] = validationProblem.Errors;

            return AspNetResults.Problem(
                type: $"urn:problem:{problem.Code}",
                title: problem.Title,
                statusCode: MapStatusCode(problem.Kind),
                detail: problem.Detail,
                instance: httpContext?.Request.Path.Value,
                extensions: extensions);
        }

        private static int MapStatusCode(ProblemKind kind) => kind switch
        {
            ProblemKind.Validation => StatusCodes.Status400BadRequest,
            ProblemKind.Unauthorized => StatusCodes.Status401Unauthorized,
            ProblemKind.Forbidden => StatusCodes.Status403Forbidden,
            ProblemKind.NotFound => StatusCodes.Status404NotFound,
            ProblemKind.Conflict => StatusCodes.Status409Conflict,
            ProblemKind.Unexpected => StatusCodes.Status500InternalServerError,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown problem kind.")
        };
    }
}
