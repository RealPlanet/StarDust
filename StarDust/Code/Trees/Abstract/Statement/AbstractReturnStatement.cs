
using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Nodes;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Statements
{
    internal sealed class AbstractReturnStatement
        : AbstractStatement
    {
        public AbstractExpression? Expression { get; }
        public override AbstractNodeType NodeType => AbstractNodeType.RETURN_STATEMENT;

        public AbstractReturnStatement(Node syntax, AbstractExpression? expression)
            : base(syntax)
        {
            Expression = expression;
        }
    }
}
