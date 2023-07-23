using StarDust.Code.AST.Data;
using StarDust.Code.AST.Nodes;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Expressions
{
    /// <summary>
    /// Rapresents an Expression within the Bound Syntax Tree
    /// </summary>
    internal abstract class AbstractExpression
        : AbstractNode
    {
        /// <summary>
        /// The type of the result of this expression
        /// </summary>
        public abstract TypeSymbol Type { get; }

        /// <summary>
        /// A constant representation of this expression if this expression could be lowered into a one. By default its null.
        /// </summary>
        public virtual AbstractConstant? ConstantValue => null;

        protected AbstractExpression(Node syntax)
            : base(syntax) { }
    }
}
