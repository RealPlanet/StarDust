﻿namespace StarDust.Code.Symbols
{
    public sealed class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol Error = new("?");
        public static readonly TypeSymbol Any = new("any");
        public static readonly TypeSymbol Bool = new("bool");
        public static readonly TypeSymbol Int = new("int");
        public static readonly TypeSymbol String = new("string");
        public static readonly TypeSymbol Void = new("void");

        public override SymbolType SymbolType => SymbolType.TYPE;
        private TypeSymbol(string name)
            : base(name)
        {
        }
    }
}
