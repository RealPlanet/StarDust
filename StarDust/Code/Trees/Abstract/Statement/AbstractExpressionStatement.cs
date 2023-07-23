using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Nodes;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Statements
{
    internal sealed class AbstractExpressionStatement
        : AbstractStatement
    {
        public AbstractExpression Expression { get; }
        public override AbstractNodeType NodeType => AbstractNodeType.EXPRESSION_STATEMENT;

        public AbstractExpressionStatement(Node syntax, AbstractExpression expression)
            : base(syntax)
        {
            Expression = expression;
        }
    }
}
