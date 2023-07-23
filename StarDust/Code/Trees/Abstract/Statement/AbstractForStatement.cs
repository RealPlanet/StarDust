using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Nodes;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;

namespace StarDust.Code.AST.Statements
{
    internal sealed class AbstractForStatement
        : AbstractLoopStatement
    {
        public override AbstractNodeType NodeType => AbstractNodeType.FOR_STATEMENT;
        public VariableSymbol Variable { get; }
        public AbstractExpression LowerBound { get; }
        public AbstractExpression UpperBound { get; }
        public AbstractStatement Body { get; }

        public AbstractForStatement(Node syntax,
                                    VariableSymbol variable,
                                    AbstractExpression lowerBound,
                                    AbstractExpression upperBound,
                                    AbstractStatement body,
                                    AbstractLabel breakLabel,
                                    AbstractLabel continueLabel)

            : base(syntax, breakLabel, continueLabel)
        {
            Variable = variable;
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Body = body;
        }
    }
}
