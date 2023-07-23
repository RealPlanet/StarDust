namespace StarDust.Code.Syntax
{
    public sealed partial class ExpressionStatement : Statement
    {
        // a = 10 -> Valid
        // a + 1 -> Not valid
        // ecc...

        public Expression Expression { get; }
        public override SyntaxType SyntaxType => SyntaxType.EXPRESSION_STATEMENT;
        internal ExpressionStatement(ConcreteSyntaxTree syntaxTree, Expression expression) : base(syntaxTree)
        {
            Expression = expression;
        }
    }
}
