using StarDust.Code.AST.Expressions;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Nodes
{
    internal sealed class AbstractUnaryOperator
    {
        public SyntaxType SyntaxType { get; }
        public AbstractUnaryType UnaryType { get; }
        public TypeSymbol OperandType { get; }
        public TypeSymbol ResultType { get; }
        private static readonly AbstractUnaryOperator[] Operators =
        {
            new AbstractUnaryOperator(SyntaxType.BANG_TOKEN, AbstractUnaryType.LOGICAL_NEGATION, TypeSymbol.Bool),
            new AbstractUnaryOperator(SyntaxType.PLUS_TOKEN, AbstractUnaryType.IDENTITY, TypeSymbol.Int),
            new AbstractUnaryOperator(SyntaxType.MINUS_TOKEN, AbstractUnaryType.NEGATION, TypeSymbol.Int),
            new AbstractUnaryOperator(SyntaxType.TILDE_TOKEN, AbstractUnaryType.ONES_COMPLEMENT, TypeSymbol.Int),
        };
        private AbstractUnaryOperator(SyntaxType tokenType, AbstractUnaryType unaryType, TypeSymbol operandType, TypeSymbol resultType)
        {
            SyntaxType = tokenType;
            UnaryType = unaryType;
            OperandType = operandType;
            ResultType = resultType;
        }
        private AbstractUnaryOperator(SyntaxType tokenType, AbstractUnaryType unaryType, TypeSymbol operandType)
            : this(tokenType, unaryType, operandType, operandType) { }
        public static AbstractUnaryOperator? Bind(SyntaxType tokenType, TypeSymbol operandType)
        {
            foreach (AbstractUnaryOperator? op in Operators)
            {
                if (op.OperandType == operandType && op.SyntaxType == tokenType)
                {
                    return op;
                }
            }

            return null;
        }
    }
}
