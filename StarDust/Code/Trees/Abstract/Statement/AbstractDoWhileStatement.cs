using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Nodes;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Statements
{
    internal sealed class AbstractDoWhileStatement
        : AbstractLoopStatement
    {
        public override AbstractNodeType NodeType => AbstractNodeType.DO_WHILE_STATEMENT;
        public AbstractStatement Body { get; }
        public AbstractExpression Condition { get; }

        public AbstractDoWhileStatement(
            Node syntax,
            AbstractStatement body,
            AbstractExpression condition,
            AbstractLabel breakLabel,
            AbstractLabel continueLabel)
            : base(syntax, breakLabel, continueLabel)
        {
            Body = body;
            Condition = condition;
        }
    }
}
