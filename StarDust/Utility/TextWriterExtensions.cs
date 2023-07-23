
using StarDust.Code.Extensions;
using StarDust.Code.Syntax;
using StarDust.Code.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace StarDust.Code.IO
{
    public static class TextWriterExtensions
    {
        private static bool IsConsole(this TextWriter writer)
        {
            if (writer == Console.Out)
                return !Console.IsOutputRedirected;

            // Color codes are always output to Console.Out
            if (writer == Console.Error)
                return !Console.IsErrorRedirected && !Console.IsOutputRedirected;

            return writer is IndentedTextWriter iw && iw.InnerWriter.IsConsole();
        }

        private static void SetForeground(this TextWriter writer, ConsoleColor consoleColor)
        {
            if (writer.IsConsole())
            {
                Console.ForegroundColor = consoleColor;
            }
        }

        private static void ResetColor(this TextWriter writer)
        {
            if (writer.IsConsole())
            {
                Console.ResetColor();
            }
        }

        public static void WriteKeyword(this TextWriter writer, SyntaxType type)
        {
            string? text = type.GetText();
            Debug.Assert(type.IsKeyword() && text is not null);
            writer.WriteKeyword(text);
        }

        public static void WriteKeyword(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.Blue);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteIdentifier(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.DarkYellow);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteNumber(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.Cyan);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteString(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.Magenta);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteSpace(this TextWriter writer)
        {
            writer.WritePunctuation(" ");
        }

        public static void WriteComment(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.DarkGreen);
            writer.Write("// " + text);
            writer.ResetColor();
        }

        public static void WritePunctuation(this TextWriter writer, SyntaxType type)
        {
            string? text = type.GetText();
            Debug.Assert(text is not null);
            writer.WritePunctuation(text);
        }

        public static void WritePunctuation(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.DarkGray);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteReport(this TextWriter writer, IEnumerable<Report> report)
        {
            foreach (Report? msg in report.Where(d => d.Location.Text is null))
            {
                ConsoleColor messageColor = msg.IsWarning ? ConsoleColor.DarkYellow : ConsoleColor.DarkRed;
                writer.SetForeground(messageColor);
                writer.WriteLine(msg);
                writer.ResetColor();
            }

            foreach (Report? msg in report.Where(d => d.Location.Text is not null)
                                    .OrderBy(d => d.Location.Text.FileName)
                                    .ThenBy(d => d.Location.Span.Start)
                                    .ThenBy(d => d.Location.Span.Length))
            {
                SourceText text = msg.Location.Text;
                string fileName = msg.Location.FileName;
                int startLine = msg.Location.StartLine + 1;
                int startCharacter = msg.Location.StartCharacter + 1;
                int endLine = msg.Location.EndLine + 1;
                int endCharacter = msg.Location.EndCharacter + 1;

                TextSpan span = msg.Location.Span;
                int lineIndex = text.GetLineIndex(span.Start);
                int lineNumber = lineIndex + 1;
                TextLine? line = text.Lines[lineIndex];
                int character = span.Start - line.Start + 1;

                writer.WriteLine();
                ConsoleColor messageColor = msg.IsWarning ? ConsoleColor.DarkYellow : ConsoleColor.DarkRed;
                writer.SetForeground(messageColor);
                writer.Write($"{fileName}({startLine},{startCharacter},{endLine},{endCharacter}): ");
                writer.WriteLine(msg);
                writer.ResetColor();

                TextSpan prefixSpan = TextSpan.FromBounds(line.Start, span.Start);
                TextSpan suffixSpan = TextSpan.FromBounds(span.End, line.End);

                string? prefix = text.ToString(prefixSpan);
                string? errorText = text.ToString(span);
                string? suffix = text.ToString(suffixSpan);

                writer.Write("    ");
                writer.Write(prefix);
                writer.SetForeground(ConsoleColor.DarkRed);
                writer.Write(errorText);
                writer.ResetColor();

                writer.WriteLine(suffix);
                writer.WriteLine();
            }
        }
    }
}
