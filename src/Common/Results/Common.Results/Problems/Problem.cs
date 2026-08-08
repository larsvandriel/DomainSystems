namespace Common.Results.Problems
{
    public class Problem
    {
        public required string Code { get; init; }
        public required string Title { get; init; }
        public string? Detail { get; init; }
        public ProblemKind Kind { get; init; } = ProblemKind.Failure;

        public IReadOnlyDictionary<string, object?> Extensions { get; init; } = new Dictionary<string, object?>();
    }
}
