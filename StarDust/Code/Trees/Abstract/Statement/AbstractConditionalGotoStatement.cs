
using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Nodes;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Statements
{
    internal sealed class AbstractConditionalGotoStatement
        : AbstractStatement
    {
        public AbstractLabel Label { get; }
        public AbstractExpression Condition { get; }
        public bool JumpIfTrue { get; }
        public override AbstractNodeType NodeType => AbstractNodeType.CONDITIONAL_GOTO_STATEMENT;

        public AbstractConditionalGotoStatement(
            Node syntax,
            AbstractLabel label,
            AbstractExpression condition,
            bool jumpIfTrue = true)
            : base(syntax)
        {
            Label = label;
            Condition = condition;
            JumpIfTrue = jumpIfTrue;
        }
    }
}
