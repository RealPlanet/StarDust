using StarDust.Code.AST.Nodes;
using StarDust.Code.Syntax;
using System.Collections.Immutable;

namespace StarDust.Code.AST.Statements
{
    internal sealed class AbstractBlockStatement
        : AbstractStatement
    {
        public ImmutableArray<AbstractStatement> Statements { get; }
        public override AbstractNodeType NodeType => AbstractNodeType.BLOCK_STATEMENT;

        public AbstractBlockStatement(Node syntax, ImmutableArray<AbstractStatement> statements)
            : base(syntax)
        {
            Statements = statements;
        }
    }
}
