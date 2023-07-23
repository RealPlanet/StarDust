using StarDust.Code.Symbols;
using System.Collections.Immutable;

namespace StarDust.Code.AST.Nodes
{
    internal sealed class AbstractScope
    {
        private Dictionary<string, Symbol>? Symbols;
        public AbstractScope? Parent { get; }

        public AbstractScope(AbstractScope? parent)
        {
            Parent = parent;
            // This dictionary could be declared as not nullable but i'd rather keep a lazy allocation instead of an eager one.
            Symbols = null;
        }

        public bool TryDeclareVariable(VariableSymbol variable)
        {
            return TryDeclareSymbol(variable);
        }

        public bool TryDeclareFunction(FunctionSymbol function)
        {
            return TryDeclareSymbol(function);
        }

        private bool TryDeclareSymbol<TSymbol>(TSymbol symbol)
            where TSymbol : Symbol
        {
            if (Symbols == null)
                Symbols = new Dictionary<string, Symbol>();
            else if (Symbols.ContainsKey(symbol.Name))
                return false;

            Symbols.Add(symbol.Name, symbol);
            return true;
        }

        public Symbol? TryLookupSymbol(string name)
        {
            if (Symbols != null && Symbols.TryGetValue(name, out Symbol? symbol))
                return symbol;

            return Parent?.TryLookupSymbol(name);
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            return GetDeclaredSymbols<VariableSymbol>();
        }

        public ImmutableArray<FunctionSymbol> GetDeclaredFunctions()
        {
            return GetDeclaredSymbols<FunctionSymbol>();
        }

        private ImmutableArray<TSymbol> GetDeclaredSymbols<TSymbol>()
            where TSymbol : Symbol
        {
            if (Symbols == null)
                return ImmutableArray<TSymbol>.Empty;

            return Symbols.Values.OfType<TSymbol>().ToImmutableArray();
        }
    }
}
