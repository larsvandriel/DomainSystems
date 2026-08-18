using Common.Messaging.Abstractions.Pipelines;
using Common.Messaging.Abstractions.Requests;
using Common.Messaging.Abstractions.Validation;
using Common.Results;
using Common.Results.Problems;

namespace Common.Messaging.Async.Pipelines
{
    public sealed class ValidationBehavior<TRequest>(
        IEnumerable<IRequestValidator<TRequest>> validators) : IRequestPipelineBehavior<TRequest, Result> where TRequest : IRequest<Result>
    {
        private readonly IRequestValidator<TRequest>[] _validators = [.. validators];

        public async Task<Result> HandleAsync(
            TRequest request,
            RequestHandlerDelegate<Result> continuation,
            CancellationToken cancellationToken = default)
        {
            var validation = await RequestValidation.AggregateAsync(request, _validators, cancellationToken);

            if (validation.IsValid)
                return await continuation(cancellationToken);

            return Result.Failure(
                ProblemFactory.Validation("request.validation_failed", "One or more validation errors occurred.", validation.ToDictionary()));
        }
    }

    public sealed class ValidationBehavior<TRequest, TResult>(
        IEnumerable<IRequestValidator<TRequest>> validators)
        : IRequestPipelineBehavior<TRequest, Result<TResult>> where TRequest : IRequest<Result<TResult>>
    {
        private readonly IRequestValidator<TRequest>[] _validators = [.. validators];

        public async Task<Result<TResult>> HandleAsync(
            TRequest request,
            RequestHandlerDelegate<Result<TResult>> continuation,
            CancellationToken cancellationToken = default)
        {
            var validation = await RequestValidation.AggregateAsync(request, _validators, cancellationToken);

            if (validation.IsValid)
                return await continuation(cancellationToken);

            return Result.Failure<TResult>(
                ProblemFactory.Validation("request.validation_failed", "One or more validation errors occurred.", validation.ToDictionary()));
        }
    }

    internal static class RequestValidation
    {
        internal static async Task<ValidationResult> AggregateAsync<TRequest>(
            TRequest request,
            IEnumerable<IRequestValidator<TRequest>> validators,
            CancellationToken cancellationToken)
        {
            var combined = new ValidationResult();

            foreach (var validator in validators)
            {
                var validation = await validator.ValidateAsync(request, cancellationToken);

                combined.Merge(validation);
            }

            return combined;
        }
    }
}
