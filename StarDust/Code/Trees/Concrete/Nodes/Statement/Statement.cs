namespace StarDust.Code.Syntax
{
    public abstract class Statement : Node
    {
        private protected Statement(ConcreteSyntaxTree syntaxTree)
                : base(syntaxTree)
        {
        }
    }
}
