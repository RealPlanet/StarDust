namespace StarDust.Code.Symbols
{
    public sealed class ParameterSymbol : LocalVariableSymbol
    {
        public override SymbolType SymbolType => SymbolType.PARAMETER;
        public int OrdinalPosition { get; }
        internal ParameterSymbol(string name, TypeSymbol type, int ordinalPosition)
            : base(name, isReadOnly: true, type, null)
        {
            OrdinalPosition = ordinalPosition;
        }
    }
}
