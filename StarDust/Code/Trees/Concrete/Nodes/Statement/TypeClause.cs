namespace StarDust.Code.Syntax
{
    public sealed partial class TypeClause : Node
    {
        public override SyntaxType SyntaxType => SyntaxType.TYPE_CLAUSE;
        public Token Identifier { get; }
        internal TypeClause(ConcreteSyntaxTree syntaxTree, Token identifier)
            : base(syntaxTree)
        {
            Identifier = identifier;
        }
    }
}
