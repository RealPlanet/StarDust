using StarDust.Code.AST.Statements;
using StarDust.Code.Symbols;
using System.Collections.Immutable;

namespace StarDust.Code.AST.Nodes
{
    internal sealed class AbstractGlobalScope
    {
        public AbstractGlobalScope? Previous { get; }
        public ImmutableArray<Report> Report { get; }
        public FunctionSymbol? MainFunction { get; }
        public FunctionSymbol? ScriptFunction { get; }
        public ImmutableArray<FunctionSymbol> Functions { get; }
        public ImmutableArray<VariableSymbol> Variables { get; }
        public ImmutableArray<AbstractStatement> Statements { get; }

        public AbstractGlobalScope(AbstractGlobalScope? previous,
                                   ImmutableArray<Report> report,
                                   FunctionSymbol? mainFunction,
                                   FunctionSymbol? scriptFunction,
                                   ImmutableArray<FunctionSymbol> functions,
                                   ImmutableArray<VariableSymbol> variables,
                                   ImmutableArray<AbstractStatement> statements)
        {
            Previous = previous;
            Report = report;
            MainFunction = mainFunction;
            ScriptFunction = scriptFunction;
            Functions = functions;
            Variables = variables;
            Statements = statements;
        }
    }
}
