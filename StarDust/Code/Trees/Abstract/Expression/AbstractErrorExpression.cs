using StarDust.Code.AST.Nodes;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Expressions
{
    /// <summary>
    /// An error expression only used during the binding process.
    /// </summary>
    internal sealed class AbstractErrorExpression
        : AbstractExpression
    {
        public override TypeSymbol Type => TypeSymbol.Error;
        public override AbstractNodeType NodeType => AbstractNodeType.ERROR_EXPRESSION;
        public AbstractErrorExpression(Node syntax)
            : base(syntax) { }
    }
}