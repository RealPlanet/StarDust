namespace StarDust.Code.AST.Nodes
{
    internal sealed class AbstractLabel
    {
        public string Name { get; }
        internal AbstractLabel(string name)
        {
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
