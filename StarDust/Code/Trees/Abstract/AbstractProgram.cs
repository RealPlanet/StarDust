using StarDust.Code.AST.Statements;
using StarDust.Code.Symbols;
using System.Collections.Immutable;

namespace StarDust.Code.AST
{
    internal sealed class AbstractProgram
    {
        public AbstractProgram? Previous { get; }
        public ImmutableArray<Report> Report { get; }
        public FunctionSymbol? MainFunction { get; }
        public FunctionSymbol? ScriptFunction { get; }
        public ImmutableDictionary<FunctionSymbol, AbstractBlockStatement> Functions { get; }

        public AbstractProgram(AbstractProgram? previous, ImmutableArray<Report> report, FunctionSymbol? mainFunction, FunctionSymbol? scriptFunction, ImmutableDictionary<FunctionSymbol, AbstractBlockStatement> functions)
        {
            Previous = previous;
            Report = report;
            MainFunction = mainFunction;
            ScriptFunction = scriptFunction;
            Functions = functions;
        }
    }
}
