namespace StarDust.Code.Syntax
{
    public sealed partial class WhileStatement : Statement
    {
        public Token WhileKeyword { get; }
        public Expression Condition { get; }
        public Token CloseParenthesisToken { get; }
        public Statement Body { get; }
        public override SyntaxType SyntaxType => SyntaxType.WHILE_STATEMENT;

        public Token OpenParenthesisToken { get; }

        internal WhileStatement(
            ConcreteSyntaxTree syntaxTree,
            Token whileKeyword,
            Token openParenthesisToken,
            Expression condition,
            Token closeParenthesisToken,
            Statement body)
            : base(syntaxTree)
        {
            WhileKeyword = whileKeyword;
            Condition = condition;
            CloseParenthesisToken = closeParenthesisToken;
            Body = body;
            OpenParenthesisToken = openParenthesisToken;
        }
    }
}
