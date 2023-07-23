namespace StarDust.Code.Syntax
{
    public sealed partial class GlobalStatement : Member
    {
        public Statement Statement { get; }
        public override SyntaxType SyntaxType => SyntaxType.GLOBAL_STATEMENT;
        internal GlobalStatement(ConcreteSyntaxTree syntaxTree, Statement statement)
            : base(syntaxTree)
        {
            Statement = statement;
        }
    }
}
