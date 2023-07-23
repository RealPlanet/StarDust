using StarDust.Code.AST;
using StarDust.Code.AST.Data;

namespace StarDust.Code.Symbols
{
    public sealed class GlobalVariableSymbol : VariableSymbol
    {
        public override SymbolType SymbolType => SymbolType.GLOBAL_VARIABLE;
        internal GlobalVariableSymbol(string name, bool isReadOnly, TypeSymbol variableType, AbstractConstant? constant)
            : base(name, isReadOnly, variableType, constant)
        {
        }
    }
}
