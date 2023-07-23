namespace StarDust.Code.Syntax
{
    public sealed partial class ReturnStatement : Statement
    {
        public override SyntaxType SyntaxType => SyntaxType.RETURN_STATEMENT;

        public Token ReturnKeyword { get; }
        public Expression? Expression { get; }
        internal ReturnStatement(ConcreteSyntaxTree syntaxTree, Token returnKeyword, Expression? expression)
            : base(syntaxTree)
        {
            ReturnKeyword = returnKeyword;
            Expression = expression;
        }
    }
}
