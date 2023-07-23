using StarDust.Code.Symbols;

namespace StarDust.Code.AST.Data
{
    internal sealed class Conversion
    {
        public static readonly Conversion None = new(exists: false, isIdentity: false, isImplicit: false);
        public static readonly Conversion Identity = new(exists: true, isIdentity: true, isImplicit: true);
        public static readonly Conversion Implict = new(exists: true, isIdentity: false, isImplicit: true);
        public static readonly Conversion Explicit = new(exists: true, isIdentity: false, isImplicit: false);
        public bool Exists { get; }
        public bool IsIdentity { get; }
        public bool IsImplicit { get; }
        public bool IsExplicit => Exists && !IsImplicit;
        private Conversion(bool exists, bool isIdentity, bool isImplicit)
        {
            Exists = exists;
            IsIdentity = isIdentity;
            IsImplicit = isImplicit;
        }

        public static Conversion Classify(TypeSymbol from, TypeSymbol to)
        {
            if (from == to)
                return Identity;

            if (from != TypeSymbol.Void && to == TypeSymbol.Any)
                return Implict;

            if (from == TypeSymbol.Any && to != TypeSymbol.Void)
                return Explicit;

            if (from == TypeSymbol.Int || from == TypeSymbol.Bool)
            {
                if (to == TypeSymbol.String)
                    return Explicit;
            }

            if (from == TypeSymbol.String)
            {
                if (to == TypeSymbol.Int || to == TypeSymbol.Bool)
                    return Explicit;
            }

            return None;
        }
    }
}
