using StarDust.Code.Text;

namespace StarDust.Code.Syntax
{
    /// <summary>
    /// Trivia is associated with a token, each token can have leading and trailing trivia. Trivia can be single or multiline comments, whitespaces, ecc..
    /// </summary>
    public sealed class Trivia
    {
        public ConcreteSyntaxTree ConcreteSyntaxTree { get; }
        public SyntaxType SyntaxType { get; }
        public int Position { get; }
        public string Text { get; }
        public TextSpan Span => new(Position, Text?.Length ?? 0);

        internal Trivia(ConcreteSyntaxTree syntaxTree, SyntaxType type, int position, string text)
        {
            ConcreteSyntaxTree = syntaxTree;
            SyntaxType = type;
            Position = position;
            Text = text;
        }
    }
}
