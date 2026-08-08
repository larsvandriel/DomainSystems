using Common.Results.Problems;

namespace Common.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Problem? Problem { get; }

        protected Result(bool isSuccess, Problem? problem)
        {
            if (isSuccess && problem != null)
                throw new InvalidOperationException("A successful result cannot contain a problem.");

            if (!isSuccess && problem == null)
                throw new InvalidOperationException("A failed result must contain a problem.");

            IsSuccess = isSuccess;
            Problem = problem;
        }

        public static Result Success() => new(true, null);

        public static Result Failure(Problem problem) => new(false, problem);

        public static Result<T> Success<T>(T value) => Result<T>.CreateSuccess(value);

        public static Result<T> Failure<T>(Problem problem) => Result<T>.CreateFailure(problem);
    }

    public sealed class Result<T> : Result
    {
        private readonly T? _value;

        public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Cannot access Value when result is failed.");


        private Result(bool isSuccess, T? value, Problem? problem) : base(isSuccess, problem)
        {
            _value = value;
        }

        internal static Result<T> CreateSuccess(T value) => new(true, value, null);

        internal static Result<T> CreateFailure(Problem problem) => new(false, default, problem);

        public static implicit operator Result<T>(T value) => CreateSuccess(value);
    }
}
