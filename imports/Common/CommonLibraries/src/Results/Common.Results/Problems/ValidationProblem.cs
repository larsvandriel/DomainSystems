namespace Common.Results.Problems
{
    public sealed class ValidationProblem : Problem
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; init; } = new Dictionary<string, string[]>();
    }
}
