
using System.Collections.Immutable;

namespace StarDust.Code.Evaluation
{
    public sealed record EvaluationResult(ImmutableArray<Report> Report, object? Value);
}
