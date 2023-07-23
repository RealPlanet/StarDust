namespace StarDust.Code.Text
{
    /// <summary>
    /// Rapresents a span of text in a source file.
    /// </summary>
    public readonly struct TextSpan
    {
        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;

        public static TextSpan FromBounds(int start, int end)
        {
            return new(start, end - start);
        }

        /// <summary>
        /// Construct a TextSpawn providing the start and end positions of the span.
        /// </summary>
        /// <param name="start">Index of character in source file</param>
        /// <param name="length">>Index of character in source file</param>
        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public override string ToString()
        {
            return $"{Start}..{End}";
        }

        public bool OverlapsWith(TextSpan other)
        {
            return Start < other.End && End > other.Start;
        }
    }
}
