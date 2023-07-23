
using StarDust.Code.AST.Nodes;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Statements
{
    internal sealed class AbstractGotoStatement
        : AbstractStatement
    {
        public AbstractLabel Label { get; }
        public override AbstractNodeType NodeType => AbstractNodeType.GOTO_STATEMENT;

        public AbstractGotoStatement(Node syntax, AbstractLabel label)
            : base(syntax)
        {
            Label = label;
        }
    }
}
