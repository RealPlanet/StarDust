using StarDust.Code.Syntax;
using Xunit;

namespace StarDust.Test
{
    internal sealed class AssertingEnum : IDisposable
    {
        private readonly IEnumerator<Node> Enumerator;
        private bool HasError = false;

        private bool SetErrorState()
        {
            HasError = true;
            return false;
        }

        public AssertingEnum(Node Node)
        {
            Enumerator = Flatten(Node).GetEnumerator();
        }

        private static IEnumerable<Node> Flatten(Node Node)
        {
            Stack<Node>? NodeStack = new();
            NodeStack.Push(Node);

            while (NodeStack.Count > 0)
            {
                Node? CurrentNode = NodeStack.Pop();
                yield return CurrentNode;

                foreach (Node? Child in CurrentNode.GetChildren().Reverse())
                {
                    NodeStack.Push(Child);
                }
            }
        }

        public void AssertToken(SyntaxType TokType, string Text)
        {
            try
            {
                Assert.True(Enumerator.MoveNext());
                Token? Token = Assert.IsType<Token>(Enumerator.Current);
                Assert.Equal(Text, Token.Text);
                Assert.Equal(TokType, Token.SyntaxType);
            }
            catch when (SetErrorState())
            {
                throw;
            }
        }

        public void AssertNode(SyntaxType SyntaxType)
        {
            try
            {
                Assert.True(Enumerator.MoveNext());
                Assert.Equal(SyntaxType, Enumerator.Current.SyntaxType);
                Assert.IsNotType<Token>(Enumerator.Current);
            }
            catch when (SetErrorState())
            {
                throw;
            }
        }

        public void Dispose()
        {
            if (!HasError)
            {
                Assert.False(Enumerator.MoveNext());
            }

            Enumerator.Dispose();
        }
    }
}