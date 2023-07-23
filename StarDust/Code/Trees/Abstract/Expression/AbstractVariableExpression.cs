using StarDust.Code.AST.Data;
using StarDust.Code.AST.Nodes;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Expressions
{
    internal sealed class AbstractVariableExpression
        : AbstractExpression
    {
        public override TypeSymbol Type => Variable.Type;
        public override AbstractNodeType NodeType => AbstractNodeType.VARIABLE_EXPRESSION;
        public VariableSymbol Variable { get; }
        public override AbstractConstant? ConstantValue => Variable.Constant;

        public AbstractVariableExpression(Node syntax, VariableSymbol variable)
            : base(syntax)
        {
            Variable = variable;
        }
    }
}
