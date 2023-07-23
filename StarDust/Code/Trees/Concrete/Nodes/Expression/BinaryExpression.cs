namespace StarDust.Code.Syntax
{
    public sealed partial class BinaryExpression : Expression
    {
        public override SyntaxType SyntaxType => SyntaxType.BINARY_EXPRESSION_NODE;
        public Expression Left { get; }
        public Token OperatorToken { get; }
        public Expression Right { get; }
        internal BinaryExpression(ConcreteSyntaxTree syntaxTree, Expression lValue, Token operatorToken, Expression rValue)
            : base(syntaxTree)
        {
            Left = lValue;
            OperatorToken = operatorToken;
            Right = rValue;
        }
    }
}
