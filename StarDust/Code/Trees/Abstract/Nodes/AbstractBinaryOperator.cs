using StarDust.Code.Symbols;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Nodes
{
    internal enum AbstractBinaryType
    {
        ADDITION,
        SUBTRACTION,
        MULTIPLICATION,
        DIVISION,
        LOGICAL_AND,
        LOGICAL_OR,
        EQUALS,
        NOT_EQUALS,

        LESS_THAN,
        LESS_THAN_OR_EQUAL,
        GREATER_THAN,
        GREATER_THAN_OR_EQUAL,
        BITWISE_AND,
        BITWISE_OR,
        BITWISE_XOR
    }

    internal sealed class AbstractBinaryOperator
    {
        public SyntaxType SyntaxType { get; }
        public AbstractBinaryType BinaryType { get; }
        public TypeSymbol LeftType { get; }
        public TypeSymbol RightType { get; }
        public TypeSymbol ResultType { get; }

        private static readonly AbstractBinaryOperator[] Operators =
        {
            new AbstractBinaryOperator(SyntaxType.PLUS_TOKEN, AbstractBinaryType.ADDITION, TypeSymbol.Int),
            new AbstractBinaryOperator(SyntaxType.MINUS_TOKEN, AbstractBinaryType.SUBTRACTION, TypeSymbol.Int),
            new AbstractBinaryOperator(SyntaxType.SLASH_TOKEN, AbstractBinaryType.DIVISION, TypeSymbol.Int),
            new AbstractBinaryOperator(SyntaxType.STAR_TOKEN, AbstractBinaryType.MULTIPLICATION, TypeSymbol.Int),

            new AbstractBinaryOperator(SyntaxType.AMPERSAND_TOKEN, AbstractBinaryType.BITWISE_AND, TypeSymbol.Int),
            new AbstractBinaryOperator(SyntaxType.PIPE_TOKEN, AbstractBinaryType.BITWISE_OR, TypeSymbol.Int),
            new AbstractBinaryOperator(SyntaxType.HAT_TOKEN, AbstractBinaryType.BITWISE_XOR, TypeSymbol.Int),

            new AbstractBinaryOperator(SyntaxType.DOUBLE_EQUALS_TOKEN, AbstractBinaryType.EQUALS, TypeSymbol.Int, TypeSymbol.Bool),
            new AbstractBinaryOperator(SyntaxType.BANG_EQUALS_TOKEN, AbstractBinaryType.NOT_EQUALS, TypeSymbol.Int, TypeSymbol.Bool),
            new AbstractBinaryOperator(SyntaxType.LESS_TOKEN, AbstractBinaryType.LESS_THAN, TypeSymbol.Int, TypeSymbol.Bool),
            new AbstractBinaryOperator(SyntaxType.LESS_OR_EQUALS_TOKEN, AbstractBinaryType.LESS_THAN_OR_EQUAL, TypeSymbol.Int, TypeSymbol.Bool),
            new AbstractBinaryOperator(SyntaxType.GREATER_TOKEN, AbstractBinaryType.GREATER_THAN, TypeSymbol.Int, TypeSymbol.Bool),
            new AbstractBinaryOperator(SyntaxType.GREATER_EQUAL_TOKEN, AbstractBinaryType.GREATER_THAN_OR_EQUAL, TypeSymbol.Int, TypeSymbol.Bool),

            new AbstractBinaryOperator(SyntaxType.AMPERSAND_TOKEN, AbstractBinaryType.BITWISE_AND, TypeSymbol.Bool),
            new AbstractBinaryOperator(SyntaxType.DOUBLE_AMPERSAND_TOKEN, AbstractBinaryType.LOGICAL_AND, TypeSymbol.Bool),
            new AbstractBinaryOperator(SyntaxType.PIPE_TOKEN, AbstractBinaryType.BITWISE_OR, TypeSymbol.Bool),
            new AbstractBinaryOperator(SyntaxType.DOUBLE_PIPE_TOKEN, AbstractBinaryType.LOGICAL_OR, TypeSymbol.Bool),
            new AbstractBinaryOperator(SyntaxType.HAT_TOKEN, AbstractBinaryType.BITWISE_XOR, TypeSymbol.Bool),
            new AbstractBinaryOperator(SyntaxType.DOUBLE_EQUALS_TOKEN, AbstractBinaryType.EQUALS, TypeSymbol.Bool),
            new AbstractBinaryOperator(SyntaxType.BANG_EQUALS_TOKEN, AbstractBinaryType.NOT_EQUALS, TypeSymbol.Bool),

            // Strings
            new AbstractBinaryOperator(SyntaxType.PLUS_TOKEN, AbstractBinaryType.ADDITION, TypeSymbol.String),
            new AbstractBinaryOperator(SyntaxType.DOUBLE_EQUALS_TOKEN, AbstractBinaryType.EQUALS, TypeSymbol.String, TypeSymbol.Bool),
            new AbstractBinaryOperator(SyntaxType.BANG_EQUALS_TOKEN, AbstractBinaryType.NOT_EQUALS, TypeSymbol.String,TypeSymbol.Bool),

            new AbstractBinaryOperator(SyntaxType.DOUBLE_EQUALS_TOKEN, AbstractBinaryType.EQUALS, TypeSymbol.Any),
            new AbstractBinaryOperator(SyntaxType.BANG_EQUALS_TOKEN, AbstractBinaryType.NOT_EQUALS, TypeSymbol.Any),
        };

        private AbstractBinaryOperator(SyntaxType tokenType, AbstractBinaryType binaryType, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol resultType)
        {
            SyntaxType = tokenType;
            BinaryType = binaryType;
            LeftType = leftType;
            RightType = rightType;
            ResultType = resultType;
        }

        private AbstractBinaryOperator(SyntaxType tokenType, AbstractBinaryType binaryType, TypeSymbol type, TypeSymbol resultType)
            : this(tokenType, binaryType, type, type, resultType)
        {
        }

        private AbstractBinaryOperator(SyntaxType tokenType, AbstractBinaryType boundType, TypeSymbol type)
            : this(tokenType, boundType, type, type, type)
        { }

        public static AbstractBinaryOperator? Bind(SyntaxType tokenType, TypeSymbol leftType, TypeSymbol rightLeft)
        {
            foreach (AbstractBinaryOperator op in Operators)
            {
                if (op.LeftType == leftType && op.SyntaxType == tokenType && op.RightType == rightLeft)
                {
                    return op;
                }
            }

            return null;
        }
    }
}
