using StarDust.Code.AST.Nodes;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Expressions
{
    /// <summary>
    /// An expression which takes another expression and converts it's result into a result which has
    /// a type corresponding to the one requested.
    /// </summary>
    internal sealed class AbstractConversionExpression
        : AbstractExpression
    {
        /// <summary>
        /// The type which this conversion will result in.
        /// </summary>
        public override TypeSymbol Type { get; }

        /// <summary>
        /// The expression which will have it's result converted to the requested type.
        /// </summary>
        public AbstractExpression Expression { get; }
        public override AbstractNodeType NodeType => AbstractNodeType.CONVERSION_EXPRESSION;

        public AbstractConversionExpression(Node syntax, TypeSymbol targetType, AbstractExpression expression)
            : base(syntax)
        {
            Type = targetType;
            Expression = expression;
        }
    }
}