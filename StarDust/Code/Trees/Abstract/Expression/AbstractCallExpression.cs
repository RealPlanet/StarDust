using StarDust.Code.AST.Nodes;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;
using System.Collections.Immutable;

namespace StarDust.Code.AST.Expressions
{
    /// <summary>
    /// This expression represents a function call.
    /// </summary>
    internal sealed class AbstractCallExpression
        : AbstractExpression
    {
        public override TypeSymbol Type => Function.ReturnType;
        public override AbstractNodeType NodeType => AbstractNodeType.CALL_EXPRESSION;

        /// <summary>
        /// The function to call with this call expression.
        /// </summary>
        public FunctionSymbol Function { get; }

        /// <summary>
        /// An Immutable Array of arguments which are passed to the function to be called.
        /// </summary>
        public ImmutableArray<AbstractExpression> Arguments { get; }

        public AbstractCallExpression(Node syntax, FunctionSymbol function, ImmutableArray<AbstractExpression> arguments)
            : base(syntax)
        {
            Function = function;
            Arguments = arguments;
        }
    }
}