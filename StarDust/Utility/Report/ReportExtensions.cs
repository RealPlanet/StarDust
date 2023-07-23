using System.Collections.Immutable;

namespace StarDust.Code
{
    public static class ReportExtensions
    {
        public static bool HasErrors(this ImmutableArray<Report> diagnostics)
        {
            return diagnostics.Any(d => d.IsError);
        }

        public static bool HasErrors(this IEnumerable<Report> diagnostics)
        {
            return diagnostics.Any(d => d.IsError);
        }
    }
}
