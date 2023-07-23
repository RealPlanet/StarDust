using StarDust.Code.AST.Data;
using StarDust.Code.AST.Nodes;
using StarDust.Code.AST.Processing;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Expressions
{
    internal enum AbstractUnaryType
    {
        IDENTITY,
        NEGATION,
        LOGICAL_NEGATION,
        ONES_COMPLEMENT
    }

    internal sealed class AbstractUnaryExpression
        : AbstractExpression
    {
        public override AbstractNodeType NodeType => AbstractNodeType.UNARY_EXPRESSION;
        public override TypeSymbol Type => Operator.ResultType;
        public AbstractUnaryOperator Operator { get; }
        public AbstractExpression Operand { get; }
        public override AbstractConstant? ConstantValue { get; }

        public AbstractUnaryExpression(Node syntax, AbstractUnaryOperator op, AbstractExpression operand)
            : base(syntax)
        {
            Operator = op;
            Operand = operand;
            ConstantValue = ConstantFolding.Fold(op, operand);
        }
    }
}
