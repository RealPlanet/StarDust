using StarDust.Code.AST;
using StarDust.Code.AST.Data;

namespace StarDust.Code.Symbols
{
    public class LocalVariableSymbol : VariableSymbol
    {
        public override SymbolType SymbolType => SymbolType.LOCAL_VARIABLE;
        internal LocalVariableSymbol(
            string name,
            bool isReadOnly,
            TypeSymbol variableType,
            AbstractConstant? constant)
            : base(name, isReadOnly, variableType, constant)
        {
        }
    }
}
