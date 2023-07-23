using StarDust.Code;
using StarDust.Code.Extensions;
using StarDust.Code.Syntax;
using StarDust.Code.Text;
using StarDust.Emitter;
using StarDust.Interpreter;
using System.CodeDom.Compiler;

namespace StarDust.Compiler
{
    public static class Program
    {
        private static int Main(string[] args)
        {
            string fileName = @".\sample_plain.sdc";

            var text = File.ReadAllText(fileName, System.Text.Encoding.Unicode);
            ConcreteSyntaxTree tree = ConcreteSyntaxTree.Parse(text);
            //tree.Root.WriteTo(Console.Out);
            tree.Root.PrintNode();

            Console.WriteLine();
            Console.WriteLine("###################################");
            Console.Out.WriteReport(tree.Report);
            Console.WriteLine("###################################");
            var compilation = Compilation.Create(tree);


            var report = compilation.WriteControlFlowGraph(".\\"); 

            Console.WriteLine("###################################");
            Console.Out.WriteReport(report);
            Console.WriteLine("###################################");

            int numOfFunctions = tree.Root.GetChildren().Count(n => n.SyntaxType == SyntaxType.FUNCTION_DECLARATION);
            Console.WriteLine($"Number of functions parsed: {numOfFunctions}");
            return 0;
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

        private static bool IsConsole(this TextWriter writer)
        {
            if (writer == Console.Out)
                return !Console.IsOutputRedirected;

            if (writer == Console.Error)
                return !Console.IsErrorRedirected && !Console.IsOutputRedirected; // Color codes are always output to Console.Out

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

        private static void PrintNode(this Node node)
        {
            foreach(var n in node.GetChildren())
            {
                Console.Write(n.SyntaxTree.Text.ToString(n.FullSpan));
            }
        }
    }
}