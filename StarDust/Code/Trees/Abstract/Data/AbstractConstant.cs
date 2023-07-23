namespace StarDust.Code.AST.Data
{
    internal sealed class AbstractConstant
    {
        public object Value { get; }
        public AbstractConstant(object value)
        {
            Value = value;
        }
    }
}
