using System.Collections;
using System.Collections.Immutable;

namespace StarDust.Code.Syntax
{
    public abstract class SeparatedSyntaxList
    {
        private protected SeparatedSyntaxList() { }
        public abstract ImmutableArray<Node> GetWithSeparators();
    }

    public sealed class SeparatedSyntaxList<T> : SeparatedSyntaxList, IEnumerable<T> where T : Node
    {
        public int Count => (NodesAndSeparators.Length + 1) / 2;
        public T this[int index] => (T)NodesAndSeparators[index * 2];
        public ImmutableArray<Node> NodesAndSeparators { get; }
        internal SeparatedSyntaxList(ImmutableArray<Node> NodesAndSeparators)
        {
            this.NodesAndSeparators = NodesAndSeparators;
        }

        public Token GetSeparator(int index)
        {
            if (index < 0 || index >= Count - 1)
                throw new ArgumentException(null, nameof(index));

            return (Token)NodesAndSeparators[index * 2 + 1];
        }

        public override ImmutableArray<Node> GetWithSeparators()
        {
            return NodesAndSeparators;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
