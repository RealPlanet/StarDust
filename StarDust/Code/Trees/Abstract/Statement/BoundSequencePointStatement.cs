using StarDust.Code.AST.Nodes;
using StarDust.Code.Syntax;
using StarDust.Code.Text;

namespace StarDust.Code.AST.Statements
{
    internal sealed class BoundSequencePointStatement
        : AbstractStatement
    {
        public override AbstractNodeType NodeType => AbstractNodeType.SEQUENCE_POINT_STATEMENT;
        public AbstractStatement Statement { get; }
        public TextLocation Location { get; }

        public BoundSequencePointStatement(Node syntax, AbstractStatement statement, TextLocation location)
            : base(syntax)
        {
            Statement = statement;
            Location = location;
        }
    }
}
