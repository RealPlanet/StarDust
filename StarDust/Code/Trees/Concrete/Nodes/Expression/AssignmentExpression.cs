namespace StarDust.Code.Syntax
{
    public sealed partial class AssignmentExpression : Expression
    {
        public Token IdentifierToken { get; }
        public Token AssignmentToken { get; }
        public Expression Expression { get; }
        public override SyntaxType SyntaxType => SyntaxType.ASSIGNMENT_EXPRESSION_NODE;

        public AssignmentExpression(ConcreteSyntaxTree syntaxTree, Token identifierToken, Token equalsToken, Expression expression)
            : base(syntaxTree)
        {
            IdentifierToken = identifierToken;
            AssignmentToken = equalsToken;
            Expression = expression;
        }
    }
}
