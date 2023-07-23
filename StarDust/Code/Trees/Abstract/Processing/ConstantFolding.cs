using StarDust.Code.AST.Data;
using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Nodes;
using StarDust.Code.Symbols;

namespace StarDust.Code.AST.Processing
{
    internal static class ConstantFolding
    {
        public static AbstractConstant? Fold(AbstractUnaryOperator op, AbstractExpression operand)
        {
            if (operand.ConstantValue is not null)
            {
                return op.UnaryType switch
                {
                    AbstractUnaryType.NEGATION => new(-(int)operand.ConstantValue.Value),
                    AbstractUnaryType.IDENTITY => new((int)operand.ConstantValue.Value),
                    AbstractUnaryType.LOGICAL_NEGATION => new(!(bool)operand.ConstantValue.Value),
                    AbstractUnaryType.ONES_COMPLEMENT => new(~(int)operand.ConstantValue.Value),
                    _ => throw new Exception($"Unexpected unary operator <{op.UnaryType}>"),
                };
            }

            return null;
        }

        public static AbstractConstant? Fold(AbstractExpression left, AbstractBinaryOperator op, AbstractExpression right)
        {
            AbstractConstant? leftConstant = left.ConstantValue;
            AbstractConstant? rightConstant = right.ConstantValue;

            #region AND/OR Short-circuit
            // Special cases that CAN be computed with partial values (AND / OR)
            // If either side of the binary operation is false then AND will result false
            if (op.BinaryType == AbstractBinaryType.LOGICAL_AND)
            {
                if (leftConstant != null && !(bool)leftConstant.Value ||
                    rightConstant != null && !(bool)rightConstant.Value)
                {
                    return new AbstractConstant(false);
                }
            }

            // If either side of the binary operation is true then OR will result true
            if (op.BinaryType == AbstractBinaryType.LOGICAL_AND)
            {
                if (leftConstant != null && (bool)leftConstant.Value ||
                    rightConstant != null && (bool)rightConstant.Value)
                {
                    return new AbstractConstant(true);
                }
            }
            #endregion

            if (leftConstant is null || rightConstant is null)
                return null;

            // We know both values are not null
            object? l = left.ConstantValue!.Value;
            object? r = right.ConstantValue!.Value;
            return op.BinaryType switch
            {
                AbstractBinaryType.ADDITION => new(EvaluateAddition(op, l, r)),
                AbstractBinaryType.SUBTRACTION => new((int)l - (int)r),
                AbstractBinaryType.MULTIPLICATION => new((int)l * (int)r),
                AbstractBinaryType.DIVISION => new((int)l / (int)r),

                AbstractBinaryType.BITWISE_AND => new(EvaluateBitwiseAnd(op, l, r)),
                AbstractBinaryType.BITWISE_OR => new(EvaluateBitwiseOr(op, l, r)),
                AbstractBinaryType.BITWISE_XOR => new(EvaluateBitwiseXOR(op, l, r)),

                AbstractBinaryType.LOGICAL_AND => new((bool)l && (bool)r),
                AbstractBinaryType.LOGICAL_OR => new((bool)l || (bool)r),
                AbstractBinaryType.EQUALS => new(Equals(l, r)),
                AbstractBinaryType.NOT_EQUALS => new(!Equals(l, r)),
                AbstractBinaryType.LESS_THAN => new((int)l < (int)r),
                AbstractBinaryType.LESS_THAN_OR_EQUAL => new((int)l <= (int)r),
                AbstractBinaryType.GREATER_THAN => new((int)l > (int)r),
                AbstractBinaryType.GREATER_THAN_OR_EQUAL => new((int)l >= (int)r),
                _ => throw new Exception($"Unexpected binary operator <{op.BinaryType}>"),
            };
        }

        private static object EvaluateAddition(AbstractBinaryOperator op, object lValue, object rValue)
        {
            if (op.LeftType == TypeSymbol.Int)
                return (int)lValue + (int)rValue;
            //if (op.Type == TypeSymbol.String)
            return (string)lValue + (string)rValue;
        }

        private static object EvaluateBitwiseOr(AbstractBinaryOperator op, object lValue, object rValue)
        {
            if (op.LeftType == TypeSymbol.Int)
                return (int)lValue | (int)rValue;

            return (bool)lValue || (bool)rValue;
        }

        private static object EvaluateBitwiseXOR(AbstractBinaryOperator op, object lValue, object rValue)
        {
            if (op.LeftType == TypeSymbol.Int)
                return (int)lValue ^ (int)rValue;

            return (bool)lValue ^ (bool)rValue;
        }

        private static object EvaluateBitwiseAnd(AbstractBinaryOperator op, object lValue, object rValue)
        {
            if (op.LeftType == TypeSymbol.Int)
                return (int)lValue & (int)rValue;

            return (bool)lValue && (bool)rValue;
        }
    }
}
