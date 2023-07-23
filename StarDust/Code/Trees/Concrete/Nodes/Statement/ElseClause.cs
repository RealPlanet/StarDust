namespace StarDust.Code.Syntax
{
    public sealed partial class ElseClauseStatement : Node
    {
        public override SyntaxType SyntaxType => SyntaxType.ELSE_CLAUSE;
        public Token ElseKeyword { get; }
        public Statement ElseStatement { get; }
        internal ElseClauseStatement(ConcreteSyntaxTree syntaxTree, Token elseKeyword, Statement elseStatement)
            : base(syntaxTree)
        {
            ElseKeyword = elseKeyword;
            ElseStatement = elseStatement;
        }
    }
}
