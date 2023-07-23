
using StarDust.Code.AST.Nodes;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Statements
{
    internal sealed class AbstractLabelStatement
        : AbstractStatement
    {
        public AbstractLabel Label { get; }
        public override AbstractNodeType NodeType => AbstractNodeType.LABEL_STATEMENT;

        public AbstractLabelStatement(Node syntax, AbstractLabel label)
            : base(syntax)
        {
            Label = label;
        }
    }
}
