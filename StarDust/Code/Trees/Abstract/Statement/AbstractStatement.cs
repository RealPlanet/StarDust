using StarDust.Code.AST.Nodes;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Statements
{
    internal abstract class AbstractStatement : AbstractNode
    {
        protected AbstractStatement(Node syntax)
            : base(syntax) { }
    }
}
