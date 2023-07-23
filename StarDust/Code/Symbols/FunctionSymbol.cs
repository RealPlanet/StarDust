using StarDust.Code.Syntax;
using System.Collections.Immutable;

namespace StarDust.Code.Symbols
{
    public sealed class FunctionSymbol : Symbol
    {
        public override SymbolType SymbolType => SymbolType.FUNCTION;
        public ImmutableArray<ParameterSymbol> Parameters { get; }
        public TypeSymbol ReturnType { get; }
        public FunctionDeclaration? Declaration { get; }

        internal FunctionSymbol(string name, ImmutableArray<ParameterSymbol> parameter, TypeSymbol returnType, FunctionDeclaration? declaration = null)
            : base(name)
        {
            Parameters = parameter;
            ReturnType = returnType;
            Declaration = declaration;
        }
    }
}
