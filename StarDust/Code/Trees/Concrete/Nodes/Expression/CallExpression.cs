namespace StarDust.Code.Syntax
{
    public sealed partial class CallExpression : Expression
    {
        public override SyntaxType SyntaxType => SyntaxType.CALL_EXPRESSION_NODE;
        public Token Identifier { get; }
        public Token OpenParenthesis { get; }
        public SeparatedSyntaxList<Expression> Arguments { get; }
        public Token CloseParenthesis { get; }
        internal CallExpression(ConcreteSyntaxTree syntaxTree, Token identifier, Token openParenthesis,
                                SeparatedSyntaxList<Expression> arguments, Token closeParenthesis)
            : base(syntaxTree)
        {
            Identifier = identifier;
            OpenParenthesis = openParenthesis;
            Arguments = arguments;
            CloseParenthesis = closeParenthesis;
        }
    }
}
