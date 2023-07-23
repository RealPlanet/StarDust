namespace StarDust.Code.Syntax
{
    public sealed partial class IfStatement : Statement
    {
        public override SyntaxType SyntaxType => SyntaxType.IF_STATEMENT;
        public Token IfKeyword { get; }
        public Token OpenParenthesisToken { get; }
        public Expression Condition { get; }
        public Token CloseParenthesisToken { get; }

        public Statement ThenStatement { get; }
        public ElseClauseStatement? ElseClause { get; }

        internal IfStatement(
            ConcreteSyntaxTree syntaxTree,
            Token ifKeyword,
            Token openParenthesisToken,
            Expression condition,
            Token closeParenthesisToken,
            Statement thenStatement,
            ElseClauseStatement? elseClause)
            : base(syntaxTree)
        {
            IfKeyword = ifKeyword;
            OpenParenthesisToken = openParenthesisToken;
            Condition = condition;
            CloseParenthesisToken = closeParenthesisToken;
            ThenStatement = thenStatement;
            ElseClause = elseClause;
        }
    }
}
