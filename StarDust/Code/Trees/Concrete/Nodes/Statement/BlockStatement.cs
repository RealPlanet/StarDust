using System.Collections.Immutable;

namespace StarDust.Code.Syntax
{
    public sealed partial class BlockStatement : Statement
    {
        public override SyntaxType SyntaxType => SyntaxType.BLOCK_STATEMENT;
        public Token OpenBraceToken { get; }
        public ImmutableArray<Statement> Statements { get; }
        public Token ClosedBraceToken { get; }
        internal BlockStatement(ConcreteSyntaxTree syntaxTree, Token openBraceToken, ImmutableArray<Statement> statements, Token closedBraceToken)
            : base(syntaxTree)
        {
            OpenBraceToken = openBraceToken;
            Statements = statements;
            ClosedBraceToken = closedBraceToken;
        }
    }
}
