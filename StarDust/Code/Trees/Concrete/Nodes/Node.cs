using StarDust.Code.Text;

namespace StarDust.Code.Syntax
{
    public abstract class Node
    {
        #region Member variables
        public ConcreteSyntaxTree SyntaxTree { get; }
        public Node? Parent => SyntaxTree.GetParent(this);
        public abstract SyntaxType SyntaxType { get; }
        /// <summary>
        /// Returns the TextSpan of a node, including their children, for example this can be the length of a for loop (initialization + body)
        /// </summary>
        public virtual TextSpan Span
        {
            get
            {
                TextSpan first = GetChildren().First().Span;
                TextSpan last = GetChildren().Last().Span;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }
        /// <summary>
        /// Returns the FULL TextSpan of a node, including their children, this includes the length of the normal text span plus the length of all the trivia
        /// </summary>
        public virtual TextSpan FullSpan
        {
            get
            {
                TextSpan first = GetChildren().First().FullSpan;
                TextSpan last = GetChildren().Last().FullSpan;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }
        public TextLocation Location => new(SyntaxTree.Text, Span);
        #endregion

        private protected Node(ConcreteSyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;
        }

        public IEnumerable<Node> AncestorsAndSelf()
        {
            Node? node = this;
            while (node != null)
            {
                yield return node;
                node = node.Parent;
            }
        }

        public IEnumerable<Node> Ancestors()
        {
            return AncestorsAndSelf().Skip(1);
        }

        public void WriteTo(TextWriter writer)
        {
            PrintTree(writer, this);
        }

        private static void PrintTree(TextWriter textWriter, Node node, string indent = "", bool isLast = true)
        {
            bool toConsoleOutput = textWriter == Console.Out;
            Token? token = node as Token;

            if (toConsoleOutput)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            if (token is not null)
            {
                foreach (Trivia? trivia in token.LeadingTrivia)
                {
                    if (toConsoleOutput)
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    textWriter.Write(indent);
                    textWriter.Write("├──");

                    if (toConsoleOutput)
                        Console.ForegroundColor = ConsoleColor.DarkGreen;

                    textWriter.WriteLine($"L: {trivia.SyntaxType}");
                }
            }

            bool hasTrailingTrivia = token is not null && token.TrailingTrivia.Any();
            string tokenMarker = !hasTrailingTrivia && isLast ? "└──" : "├──";

            if (toConsoleOutput)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            textWriter.Write(indent);
            textWriter.Write(tokenMarker);

            if (toConsoleOutput)
                Console.ForegroundColor = node is Token ? ConsoleColor.Blue : ConsoleColor.Cyan;

            textWriter.Write(node.SyntaxType);

            if (token is not null && token.Value is not null)
            {
                textWriter.Write($" {token.Value}");
            }

            if (toConsoleOutput)
                Console.ResetColor();

            textWriter.WriteLine();

            if (token is not null)
            {
                foreach (Trivia? trivia in token.TrailingTrivia)
                {
                    bool isLastTrailingTrivia = trivia == token.TrailingTrivia.Last();
                    string triviaMarker = isLast && isLastTrailingTrivia ? "└──" : "├──";

                    if (toConsoleOutput)
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    textWriter.Write(indent);
                    textWriter.Write(triviaMarker);

                    if (toConsoleOutput)
                        Console.ForegroundColor = ConsoleColor.DarkGreen;

                    textWriter.WriteLine($"T: {trivia.SyntaxType}");
                }
            }

            indent += isLast ? "   " : "│  ";

            Node? lastChild = node.GetChildren().LastOrDefault();
            foreach (Node? Child in node.GetChildren())
            {
                PrintTree(textWriter, Child, indent, Child == lastChild);
            }
        }

        public abstract IEnumerable<Node> GetChildren();

        /// <summary>
        /// Returns the last leaf of this sub tree
        /// </summary>
        /// <returns>A token node/leaf</returns>
        public Token GetLastToken()
        {
            if (this is Token t)
                return t;

            return GetChildren().Last().GetLastToken();
        }

        public override string ToString()
        {
            using (StringWriter? writer = new())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }
    }
}
