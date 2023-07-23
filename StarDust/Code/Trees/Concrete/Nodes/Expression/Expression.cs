namespace StarDust.Code.Syntax
{
    public abstract class Expression : Node
    {
        private protected Expression(ConcreteSyntaxTree syntaxTree)
            : base(syntaxTree)
        {
        }
    }
}
