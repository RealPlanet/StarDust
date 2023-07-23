namespace StarDust.Code.Syntax
{
    // TODO :: Support more common for statement
    public sealed partial class ForStatement : Statement
    {
        public Token Keyword { get; }
       // public Token OpenParenthesisToken { get; }
        public Token Identifier { get; }
        public Token EqualsToken { get; }
        public Expression LowerBound { get; }
        public Token ToKeyword { get; }
        public Expression UpperBound { get; }
        //public Token CloseParenthesisToken { get; }
        public Statement Body { get; }
        public override SyntaxType SyntaxType => SyntaxType.FOR_STATEMENT;

        // Bounds are currently inclusive
        internal ForStatement(ConcreteSyntaxTree syntaxTree,
                              Token keyword,
                              /*Token openParenthesisToken,*/
                              Token identifier,
                              Token equalsToken,
                              Expression lowerBound,
                              Token toKeyword,
                              Expression upperBound,
                              /*Token closeParenthesisToken,*/
                              Statement body)
            : base(syntaxTree)
        {
            // for i = 1 to 10
            // {
            //
            // }

            Keyword = keyword;
            //OpenParenthesisToken = openParenthesisToken;
            Identifier = identifier;
            EqualsToken = equalsToken;
            LowerBound = lowerBound;
            ToKeyword = toKeyword;
            UpperBound = upperBound;
            //CloseParenthesisToken = closeParenthesisToken;
            Body = body;
        }
    }
}
