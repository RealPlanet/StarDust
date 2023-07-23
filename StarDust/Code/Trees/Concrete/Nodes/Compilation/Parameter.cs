namespace StarDust.Code.Syntax
{
    public sealed partial class Parameter : Node
    {
        public override SyntaxType SyntaxType => SyntaxType.PARAMETER;
        public TypeClause Type { get; }
        public Token Identifier { get; }
        internal Parameter(ConcreteSyntaxTree syntaxTree, TypeClause typeClause, Token identifier)
            : base(syntaxTree)
        {
            Type = typeClause;
            Identifier = identifier;
        }
    }
}
