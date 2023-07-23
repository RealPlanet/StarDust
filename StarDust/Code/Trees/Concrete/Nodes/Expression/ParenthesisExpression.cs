namespace StarDust.Code.Syntax
{
    public sealed partial class ParenthesisExpression : Expression
    {
        public override SyntaxType SyntaxType => SyntaxType.PARENTHESIZED_EXPRESSION_NODE;
        public Token OpenParenthesisToken { get; }
        public Expression Expression { get; }
        public Token ClosedParenthesisToken { get; }

        internal ParenthesisExpression(ConcreteSyntaxTree syntaxTree, Token openParenthesisToken, Expression expression, Token closedParenthesisToken)
            : base(syntaxTree)
        {
            OpenParenthesisToken = openParenthesisToken;
            Expression = expression;
            ClosedParenthesisToken = closedParenthesisToken;
        }
    }
}
