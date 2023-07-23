namespace StarDust.Code.Syntax
{
    public sealed partial class ContinueStatement : Statement
    {
        public Token Keyword { get; }
        public override SyntaxType SyntaxType => SyntaxType.CONTINUE_STATEMENT;
        internal ContinueStatement(ConcreteSyntaxTree syntaxTree, Token keyword)
            : base(syntaxTree)
        {
            Keyword = keyword;
        }
    }
}
