using StarDust.Code.AST;
using StarDust.Code.AST.Nodes;
using StarDust.Code.AST.Processing;
using StarDust.Code.AST.Statements;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;
using System.Collections.Immutable;

namespace StarDust.Code
{
    public sealed class Compilation
    {
        private AbstractGlobalScope? _GlobalScope;
        internal AbstractGlobalScope GlobalScope
        {
            get
            {
                if (_GlobalScope is null)
                {
                    AbstractGlobalScope? globalScope = AbstractSyntaxTree.BindGlobalScope(IsScript, Previous?.GlobalScope, SyntaxTrees);

                    // First thread to get here will assign the value
                    Interlocked.CompareExchange(ref _GlobalScope, globalScope, null);
                }

                return _GlobalScope;
            }
        }

        public bool IsScript { get; }
        public Compilation? Previous { get; }
        public ImmutableArray<ConcreteSyntaxTree> SyntaxTrees { get; }
        public FunctionSymbol? MainFunction => GlobalScope.MainFunction;
        public ImmutableArray<FunctionSymbol> Functions => GlobalScope.Functions;
        public ImmutableArray<VariableSymbol> Variables => GlobalScope.Variables;

        private Compilation(bool isScript, Compilation? previous, params ConcreteSyntaxTree[] syntaxTrees)
        {
            IsScript = isScript;
            Previous = previous;
            SyntaxTrees = syntaxTrees.ToImmutableArray();
        }

        public static Compilation Create(params ConcreteSyntaxTree[] syntaxTrees)
        {
            return new(false, null, syntaxTrees);
        }

        public static Compilation CreateScript(Compilation? previous, params ConcreteSyntaxTree[] syntaxTrees)
        {
            return new(true, previous, syntaxTrees);
        }

        public IEnumerable<Symbol> GetSymbols()
        {
            Compilation? submission = this;
            HashSet<string> seenSymbolNames = new();

            //List<FunctionSymbol> builtinFunctions = BuiltinFunction.GetAll().ToList();
            while (submission is not null)
            {
                foreach (FunctionSymbol? function in submission.Functions)
                {
                    if (seenSymbolNames.Add(function.Name))
                        yield return function;
                }

                foreach (VariableSymbol? variable in submission.Variables)
                {
                    if (seenSymbolNames.Add(variable.Name))
                        yield return variable;
                }

                //foreach (FunctionSymbol? builtin in builtinFunctions)
                //{
                //    if (seenSymbolNames.Add(builtin.Name))
                //        yield return builtin;
                //}

                submission = submission.Previous;
            }
        }

        public Compilation ContinueWith(ConcreteSyntaxTree syntaxTree)
        {
            return Compilation.CreateScript(this, syntaxTree);
        }

        public void WriteTree(TextWriter writer)
        {
            if (GlobalScope.MainFunction is not null)
            {
                WriteTree(GlobalScope.MainFunction, writer);
            }
            else if (GlobalScope.ScriptFunction is not null)
            {
                WriteTree(GlobalScope.ScriptFunction, writer);
            }
        }

        public void WriteTree(FunctionSymbol symbol, TextWriter writer)
        {
            AbstractProgram program = GetProgram();

            symbol.WriteTo(writer);
            writer.WriteLine();
            if (!program.Functions.TryGetValue(symbol, out AbstractBlockStatement? body))
                return;

            body.WriteTo(writer);
        }

        internal AbstractProgram GetProgram()
        {
            AbstractProgram? previous = Previous?.GetProgram();
            return AbstractSyntaxTree.BindProgram(IsScript, previous, GlobalScope);
        }
    }
}
