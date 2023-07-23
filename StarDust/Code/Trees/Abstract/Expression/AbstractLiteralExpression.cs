using StarDust.Code.AST.Data;
using StarDust.Code.AST.Nodes;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Expressions
{
    internal sealed class AbstractLiteralExpression
        : AbstractExpression
    {
        public override TypeSymbol Type { get; }
        public override AbstractNodeType NodeType => AbstractNodeType.LITERAL_EXPRESSION;
        public object Value => ConstantValue.Value;
        public override AbstractConstant ConstantValue { get; }

        public AbstractLiteralExpression(Node syntax, object value)
            : base(syntax)
        {
            Type = value switch
            {
                bool => TypeSymbol.Bool,
                int => TypeSymbol.Int,
                string => TypeSymbol.String,
                _ => throw new Exception($"Unexpected literal <{value}> of type <{value.GetType()}>"),
            };

            ConstantValue = new(value);
        }
    }
}
