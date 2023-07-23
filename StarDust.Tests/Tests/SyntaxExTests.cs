using StarDust.Code;
using StarDust.Code.Extensions;
using StarDust.Code.Syntax;
using Xunit;

namespace StarDust.Test
{
    public class SyntaxExTests
    {
        [Theory]
        [MemberData(nameof(GetSyntaxTypeData))]
        public void SyntaxFact_GetText_RoundTrips(SyntaxType Type)
        {
            string? Text = Type.GetText();
            if (Text is null)
            {
                return;
            }

            System.Collections.Immutable.ImmutableArray<Token> Tokens = ConcreteSyntaxTree.ParseTokens(Text);

            Token Token = Assert.Single(Tokens);
            Assert.Equal(Type, Token.SyntaxType);
            Assert.Equal(Text, Token.Text);
        }

        public static IEnumerable<object[]> GetSyntaxTypeData()
        {
            SyntaxType[]? Types = (SyntaxType[])Enum.GetValues(typeof(SyntaxType));
            foreach (SyntaxType v in Types)
            {
                yield return new object[] { v };
            }
        }
    }
}