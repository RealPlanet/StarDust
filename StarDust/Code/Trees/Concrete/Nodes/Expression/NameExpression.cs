namespace StarDust.Code.Syntax
{
    public sealed partial class NameExpression : Expression
    {
        public Token IdentifierToken { get; }

        public override SyntaxType SyntaxType => SyntaxType.NAME_EXPRESSION_NODE;

        internal NameExpression(ConcreteSyntaxTree syntaxTree, Token identifierToken)
            : base(syntaxTree)
        {
            IdentifierToken = identifierToken;
        }
    }
}
