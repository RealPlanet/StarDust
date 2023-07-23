namespace StarDust.Code.Syntax
{
    // var x = 10 --> Reassignable
    // const x = 10 --> Not Reassignable
    public sealed partial class VariableDeclaration : Statement
    {
        public override SyntaxType SyntaxType => SyntaxType.VARIABLE_DECLARATION;
        public Token Keyword { get; }
        public Token Identifier { get; }
        public TypeClause? Type { get; }
        public Token EqualsToken { get; }
        public Expression Initializer { get; }

        internal VariableDeclaration(ConcreteSyntaxTree syntaxTree, Token keyword, TypeClause? typeClause, Token identifier, Token equalsToken, Expression initializer) : base(syntaxTree)
        {
            Keyword = keyword;
            Type = typeClause;
            Identifier = identifier;
            EqualsToken = equalsToken;
            Initializer = initializer;
        }
    }
}
