namespace StarDust.Code.Syntax
{
    public sealed partial class BreakStatement : Statement
    {
        public Token Keyword { get; }
        public override SyntaxType SyntaxType => SyntaxType.BREAK_STATEMENT;
        internal BreakStatement(ConcreteSyntaxTree syntaxTree, Token keyword)
            : base(syntaxTree)
        {
            Keyword = keyword;
        }
    }
}
