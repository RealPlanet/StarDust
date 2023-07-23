using StarDust.Code.AST.Nodes;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Statements
{
    internal abstract class AbstractLoopStatement
        : AbstractStatement
    {
        public AbstractLabel BreakLabel { get; }
        public AbstractLabel ContinueLabel { get; }

        protected AbstractLoopStatement(Node syntax, AbstractLabel breakLabel, AbstractLabel continueLabel)
            : base(syntax)
        {
            BreakLabel = breakLabel;
            ContinueLabel = continueLabel;
        }
    }
}
