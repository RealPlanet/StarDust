using StarDust.Code.AST.Nodes;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Expressions.Expressions
{
    internal sealed class AbstractCompoundAssignmentExpression
        : AbstractExpression
    {
        public override AbstractNodeType NodeType => AbstractNodeType.COMPOUND_ASSIGNMENT_EXPRESSION;
        public override TypeSymbol Type => Expression.Type;
        public VariableSymbol Variable { get; }
        public AbstractBinaryOperator Operator { get; }
        public AbstractExpression Expression { get; }

        public AbstractCompoundAssignmentExpression(Node syntax, VariableSymbol variable, AbstractBinaryOperator op, AbstractExpression expression)
            : base(syntax)
        {
            Variable = variable;
            Operator = op;
            Expression = expression;
        }
    }
}
