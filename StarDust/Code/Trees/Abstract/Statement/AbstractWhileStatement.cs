using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Nodes;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Statements
{

    internal sealed class AbstractWhileStatement
        : AbstractLoopStatement
    {
        public override AbstractNodeType NodeType => AbstractNodeType.WHILE_STATEMENT;
        public AbstractExpression Condition { get; }
        public AbstractStatement Body { get; }

        public AbstractWhileStatement(Node syntax, AbstractExpression condition, AbstractStatement body,
            AbstractLabel breakLabel, AbstractLabel continueLabel)
            : base(syntax, breakLabel, continueLabel)
        {
            Condition = condition;
            Body = body;
        }
    }
}
