using StarDust.Code.AST;
using StarDust.Code.AST.Data;

namespace StarDust.Code.Symbols
{
    public abstract class VariableSymbol : Symbol
    {
        public bool IsReadOnly { get; }
        public TypeSymbol Type { get; }
        internal AbstractConstant? Constant { get; }
        internal VariableSymbol(string Name, bool isReadOnly, TypeSymbol variableType, AbstractConstant? constant)
            : base(Name)
        {
            IsReadOnly = isReadOnly;
            Type = variableType;
            Constant = IsReadOnly ? constant : null;
        }
    }
}
