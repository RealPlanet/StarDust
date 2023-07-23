using System.Collections.Immutable;

namespace StarDust.Code.Text
{
    public sealed class SourceText
    {
        public ImmutableArray<TextLine> Lines;
        public string Text { get; }
        public string FileName { get; }

        private SourceText(string text, string fileName)
        {
            Lines = ParseLines(this, text);
            Text = text;
            FileName = fileName;
        }

        public char this[int index] => Text[index];
        public int Length => Text.Length;


        public static SourceText From(string text, string fileName = "")
        {
            return new(text, fileName);
        }

        public int GetLineIndex(int position)
        {
            int lower = 0, upper = Lines.Length - 1;
            while (lower <= upper)
            {
                int index = lower + (upper - lower) / 2;
                int start = Lines[index].Start;

                if (start == position)
                {
                    return index;
                }

                if (start > position)
                {
                    upper = index - 1;
                    continue;
                }

                lower = index + 1;
            }

            return lower - 1;
        }

        private static ImmutableArray<TextLine> ParseLines(SourceText sourceText, string text)
        {
            ImmutableArray<TextLine>.Builder? result = ImmutableArray.CreateBuilder<TextLine>();
            int lineStart = 0, position = 0;

            while (position < text.Length)
            {
                int lineBreakWidth = GetLineBreakWidth(text, position);
                if (lineBreakWidth == 0)
                {
                    position++;
                    continue;
                }

                AddLine(result, sourceText, position, lineStart, lineBreakWidth);
                position += lineBreakWidth;
                lineStart = position;
            }

            if (position >= lineStart)
            {
                AddLine(result, sourceText, position, lineStart, 0);
            }

            return result.ToImmutable();
        }

        private static void AddLine(ImmutableArray<TextLine>.Builder result, SourceText sourceText, int position, int lineStart, int lineBreakWidth)
        {
            int lineLength = position - lineStart;
            int lineLengthIncludingLineBreak = lineLength + lineBreakWidth;
            TextLine? line = new(sourceText, lineStart, lineLength, lineLengthIncludingLineBreak);
            result.Add(line);
        }

        private static int GetLineBreakWidth(string text, int position)
        {
            char c = text[position];
            char l = position + 1 >= text.Length ? '\0' : text[position + 1];
            if (c == '\r' && l == '\n')
            {
                return 2;
            }

            if (c == '\r' || l == '\n')
            {
                return 1;
            }

            return 0;
        }

        public override string ToString()
        {
            return Text;
        }

        public string ToString(int start, int length)
        {
            return Text.Substring(start, length);
        }

        public string ToString(TextSpan span)
        {
            return ToString(span.Start, span.Length);
        }
    }
}
