namespace Common.Results
{
    public static class ResultExtensions
    {
        public static Result<TTarget> MapFailure<TTarget>(this Result result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot convert a successful Result to a failure result.");

            return Result.Failure<TTarget>(result.Problem!);
        }

        public static Result<TTarget> MapFailure<TSource, TTarget>(this Result<TSource> result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot convert a successful Result to a failure result.");

            return Result.Failure<TTarget>(result.Problem!);
        }
    }
}
