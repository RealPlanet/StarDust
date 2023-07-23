using StarDust.Code.AST.Nodes;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Expressions
{
    /// <summary>
    /// An assignment expression withing Bound Syntax Tree.
    /// </summary>
    internal sealed class AbstractAssignmentExpression
        : AbstractExpression
    {
        public override TypeSymbol Type => Expression.Type;
        public override AbstractNodeType NodeType => AbstractNodeType.ASSIGNMENT_EXPRESSION;

        /// <summary>
        /// The variable to which assign the result of the Expression.
        /// </summary>
        public VariableSymbol Variable { get; }

        /// <summary>
        /// The R-Value of this assignment which can be any type of expresion which returns a result.
        /// </summary>
        public AbstractExpression Expression { get; }

        public AbstractAssignmentExpression(Node syntax, VariableSymbol variable, AbstractExpression expression)
            : base(syntax)
        {
            Variable = variable;
            Expression = expression;
        }
    }
}