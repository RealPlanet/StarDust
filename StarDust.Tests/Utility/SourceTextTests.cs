using StarDust.Code.Text;
using Xunit;

namespace StarDust.Test
{
    public class SourceTextTests
    {
        [Theory]
        [InlineData(".", 1)]
        [InlineData(".\r\n", 2)]
        [InlineData(".\r\n\r\n", 3)]
        public void SourceText_IncludesLastLine(string text, int expectedLineCount)
        {
            SourceText source = SourceText.From(text);
            Assert.Equal(expectedLineCount, source.Lines.Length);
        }
    }
}