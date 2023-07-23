using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Nodes;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Statements
{
    internal sealed class AbstractIfStatement
        : AbstractStatement
    {
        public AbstractExpression Condition { get; }
        public AbstractStatement ThenStatement { get; }
        public AbstractStatement? ElseStatement { get; }
        public override AbstractNodeType NodeType => AbstractNodeType.IF_STATEMENT;
        public AbstractIfStatement(Node syntax, AbstractExpression condition, AbstractStatement thenStatement, AbstractStatement? elseStatement)
            : base(syntax)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElseStatement = elseStatement;
        }
    }
}
