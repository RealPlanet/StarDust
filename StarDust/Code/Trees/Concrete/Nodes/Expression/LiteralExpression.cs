namespace StarDust.Code.Syntax
{
    public sealed partial class LiteralExpression : Expression
    {
        public override SyntaxType SyntaxType => SyntaxType.LITERAL_EXPRESSION_NODE;
        public Token LiteralToken { get; }
        public object Value { get; }

        internal LiteralExpression(ConcreteSyntaxTree syntaxTree, Token literalToken, object value)
            : base(syntaxTree)
        {
            LiteralToken = literalToken;
            Value = value;
        }

        internal LiteralExpression(ConcreteSyntaxTree syntaxTree, Token literalToken)
            : this(syntaxTree, literalToken, literalToken.Value!)
        {
        }
    }
}
