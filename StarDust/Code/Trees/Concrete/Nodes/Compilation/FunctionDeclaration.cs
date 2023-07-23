namespace StarDust.Code.Syntax
{
    public sealed partial class FunctionDeclaration : Member
    {
        public override SyntaxType SyntaxType => SyntaxType.FUNCTION_DECLARATION;
        public Token FunctionKeyWord { get; }
        public TypeClause? Type { get; }
        public Token Identifier { get; }
        public Token OpenParenthesisToken { get; }
        public SeparatedSyntaxList<Parameter> Parameters { get; }
        public Token ClosedParenthesisToken { get; }
        public Statement Body { get; }
        internal FunctionDeclaration(ConcreteSyntaxTree syntaxTree, Token functionKeyWord, TypeClause? type,
                                    Token identifier, Token openParenthesisToken, SeparatedSyntaxList<Parameter> parameters,
                                    Token closedParenthesisToken, Statement body)
            : base(syntaxTree)
        {
            FunctionKeyWord = functionKeyWord;
            Type = type;
            Identifier = identifier;
            OpenParenthesisToken = openParenthesisToken;
            Parameters = parameters;
            ClosedParenthesisToken = closedParenthesisToken;
            Body = body;
        }
    }
}
