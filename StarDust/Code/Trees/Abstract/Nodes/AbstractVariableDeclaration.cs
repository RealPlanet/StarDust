using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Statements;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Nodes
{
    internal sealed class AbstractVariableDeclaration
        : AbstractStatement
    {
        public VariableSymbol Variable { get; }
        public AbstractExpression Initializer { get; }
        public override AbstractNodeType NodeType => AbstractNodeType.VARIABLE_DECLARATION_STATEMENT;

        public AbstractVariableDeclaration(Node syntax, VariableSymbol variable, AbstractExpression initializer)
            : base(syntax)
        {
            Variable = variable;
            Initializer = initializer;
        }
    }
}
