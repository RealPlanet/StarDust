using StarDust.Code.AST.Nodes;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Statements
{
    internal sealed class AbstractNopStatement
        : AbstractStatement
    {
        public override AbstractNodeType NodeType => AbstractNodeType.NOP_STATEMENT;

        public AbstractNopStatement(Node syntax)
            : base(syntax) { }
    }
}
