namespace StarDust.Code.Syntax
{
    public abstract partial class Member : Node
    {
        private protected Member(ConcreteSyntaxTree syntaxTree)
            : base(syntaxTree)
        {
        }
    }
}
