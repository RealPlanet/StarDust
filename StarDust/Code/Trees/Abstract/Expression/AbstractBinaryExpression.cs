using StarDust.Code.AST.Data;
using StarDust.Code.AST.Nodes;
using StarDust.Code.AST.Processing;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Expressions
{
    /// <summary>
    /// An expression which takes two generic BoundExpression nodes and an operator node and produces a result.<br/>
    /// For example: 1 + 1.
    /// </summary>
    internal sealed class AbstractBinaryExpression
        : AbstractExpression
    {
        public override AbstractNodeType NodeType => AbstractNodeType.BINARY_EXPRESSION;
        public override TypeSymbol Type => Operator.ResultType;

        /// <summary>
        /// The left expression of this binary node.
        /// </summary>
        public AbstractExpression Left { get; }

        /// <summary>
        /// The operator of this binary node which dictates the behaviour of this operation.
        /// </summary>
        public AbstractBinaryOperator Operator { get; }

        /// <summary>
        /// The right expression of this binary node.
        /// </summary>
        public AbstractExpression Right { get; }
        public override AbstractConstant? ConstantValue { get; }

        public AbstractBinaryExpression(Node syntax, AbstractExpression left, AbstractBinaryOperator op, AbstractExpression right)
            : base(syntax)
        {
            Left = left;
            Operator = op;
            Right = right;
            // Check if this expression can be folded into a constant
            ConstantValue = ConstantFolding.Fold(left, op, right);
        }
    }
}
