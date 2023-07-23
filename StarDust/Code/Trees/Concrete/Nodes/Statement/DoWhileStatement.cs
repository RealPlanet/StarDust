namespace StarDust.Code.Syntax
{
    public sealed partial class DoWhileStatement : Statement
    {
        public Token DoKeyword { get; }
        public Statement Body { get; }
        public Token WhileKeyword { get; }
        public Token OpenParenthesisToken { get; }
        public Expression Condition { get; }
        public Token CloseParenthesisToken { get; }

        public override SyntaxType SyntaxType => SyntaxType.DO_WHILE_STATEMENT;
        internal DoWhileStatement(
            ConcreteSyntaxTree syntaxTree,
            Token doKeyword,
            Statement body,
            Token whileKeyword,
            Token openParenthesisToken,
            Expression condition,
            Token closeParenthesisToken)
            : base(syntaxTree)
        {
            DoKeyword = doKeyword;
            Body = body;
            WhileKeyword = whileKeyword;
            OpenParenthesisToken = openParenthesisToken;
            Condition = condition;
            CloseParenthesisToken = closeParenthesisToken;
        }
    }
}
