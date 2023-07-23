using StarDust.Code.Text;
using System.Collections.Immutable;

namespace StarDust.Code.Syntax
{
    public sealed class Token : Node
    {
        #region Member Variables
        public override SyntaxType SyntaxType { get; }
        public override TextSpan Span => new(TextPosition, Text.Length);
        public override TextSpan FullSpan
        {
            get
            {
                int start = LeadingTrivia.Length == 0
                    ? Span.Start
                    : LeadingTrivia.First().Span.Start;

                int end = TrailingTrivia.Length == 0
                    ? Span.End
                    : TrailingTrivia.Last().Span.End;
                return TextSpan.FromBounds(start, end);
            }
        }
        public int TextPosition { get; } = -1;
        /// <summary>
        /// A token contains text which either is the keyword text (for, do, while, etc..) OR the value of the token (literal, variable name, etc..)
        /// </summary>
        public string Text { get; } = string.Empty;
        public object? Value { get; }
        /// <summary>
        /// A token is missing if it was inserted by the parser and doesn't appear in the source.
        /// </summary>
        public bool IsMissing { get; }
        public ImmutableArray<Trivia> LeadingTrivia { get; }
        public ImmutableArray<Trivia> TrailingTrivia { get; }
        #endregion
        internal Token(ConcreteSyntaxTree syntaxTree, SyntaxType type, int position, string? text, object? value, ImmutableArray<Trivia> leadingTrivia, ImmutableArray<Trivia> trailingTrivia)
            : base(syntaxTree)
        {
            SyntaxType = type;
            TextPosition = position;

            Text = text ?? string.Empty;
            IsMissing = text is null;

            Value = value;
            LeadingTrivia = leadingTrivia;
            TrailingTrivia = trailingTrivia;
        }

        public override IEnumerable<Node> GetChildren()
        {
            return Array.Empty<Node>();
        }
    }
}
