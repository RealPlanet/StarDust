namespace StarDust.Code.Syntax
{
    /// <summary>
    /// Rapresents a unary operation in the Syntax Tree such as -1.
    /// In the case of -1, the operator is the minus sign and the operand is 1.
    /// </summary>
    public sealed partial class UnaryExpression : Expression
    {
        public override SyntaxType SyntaxType => SyntaxType.UNARY_EXPRESSION_NODE;
        public Token OperatorToken { get; }
        public Expression Operand { get; }

        internal UnaryExpression(ConcreteSyntaxTree syntaxTree, Token operatorToken, Expression operand)
            : base(syntaxTree)
        {
            Operand = operand;
            OperatorToken = operatorToken;
        }
    }
}
