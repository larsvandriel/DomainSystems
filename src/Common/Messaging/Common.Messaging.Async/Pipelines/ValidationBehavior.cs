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
            var errors = await RequestValidation.ValidateAsync(request, _validators, cancellationToken);

            if (!errors.Any)
                return await continuation(cancellationToken);

            return Result.Failure(
                ProblemFactory.Validation("request.validation_failed", "One or more validation errors occurred.", errors.ToDictionary()));
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
            var validationErrors = await RequestValidation.ValidateAsync(request, _validators, cancellationToken);

            if (!validationErrors.Any)
                return await continuation(cancellationToken);

            return Result.Failure<TResult>(
                ProblemFactory.Validation("request.validation_failed", "One or more validation errors occurred.", validationErrors.ToDictionary()));
        }
    }

    internal static class RequestValidation
    {
        internal static async Task<ValidationErrors> ValidateAsync<TRequest>(
            TRequest request,
            IEnumerable<IRequestValidator<TRequest>> validators,
            CancellationToken cancellationToken)
        {
            var errors = new ValidationErrors();

            foreach (var validator in validators)
            {
                var failures = await validator.ValidateAsync(request, cancellationToken);

                foreach (var failure in failures)
                {
                    errors.Add(failure.PropertyName, failure.ErrorMessage);
                }
            }

            return errors;
        }
    }
}
