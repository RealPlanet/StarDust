using StarDust.Code.Text;
using System.Collections.Immutable;

namespace StarDust.Code.Syntax
{
    public sealed class ConcreteSyntaxTree
    {
        private Dictionary<Node, Node?>? Parents;

        private delegate void ParseHandler(ConcreteSyntaxTree syntaxTree, out CompilationUnit root, out ImmutableArray<Report> report);
        public SourceText Text { get; }
        public ImmutableArray<Report> Report { get; }
        public CompilationUnit Root { get; }
        private ConcreteSyntaxTree(SourceText text, ParseHandler handler)
        {
            Text = text;
            handler(this, out CompilationUnit? root, out ImmutableArray<Report> report);

            Report = report;
            Root = root;
        }

        public static ConcreteSyntaxTree Load(string fileName)
        {
            string? text = File.ReadAllText(fileName);
            SourceText? sourceText = SourceText.From(text, fileName);
            return Parse(sourceText);
        }
        private static void Parse(ConcreteSyntaxTree syntaxTree, out CompilationUnit root, out ImmutableArray<Report> report)
        {
            Parser parser = new(syntaxTree);
            root = parser.ParseCompilationUnit();
            report = parser.Report.ToImmutableArray();
        }

        public static ConcreteSyntaxTree Parse(string text)
        {
            SourceText sourceText = SourceText.From(text);
            return Parse(sourceText);
        }
        public static ConcreteSyntaxTree Parse(SourceText text)
        {
            return new(text, Parse);
        }

        public static ImmutableArray<Token> ParseTokens(string text, bool includeEOF = false)
        {
            SourceText sourceText = SourceText.From(text);
            return ParseTokens(sourceText, includeEOF);
        }
        public static ImmutableArray<Token> ParseTokens(string text, out ImmutableArray<Report> report, bool includeEOF = false)
        {
            SourceText sourceText = SourceText.From(text);
            return ParseTokens(sourceText, out report, includeEOF);
        }
        public static ImmutableArray<Token> ParseTokens(SourceText text, bool includeEOF = false)
        {
            return ParseTokens(text, out _, includeEOF);
        }

        public static ImmutableArray<Token> ParseTokens(SourceText text, out ImmutableArray<Report> report, bool includeEOF)
        {
            List<Token> tokens = new();
            void ParseTokens(ConcreteSyntaxTree syntaxTree, out CompilationUnit root, out ImmutableArray<Report> r)
            {
                Lexer lexer = new(syntaxTree);
                while (true)
                {
                    Token tok = lexer.Lex();
                    bool isEOF = tok.SyntaxType == SyntaxType.END_OF_FILE_TOKEN;
                    if (isEOF)
                    {
                        root = new(syntaxTree, ImmutableArray<Member>.Empty, tok);

                        if (includeEOF)
                            tokens.Add(tok);

                        break;
                    }

                    if (!isEOF)
                        tokens.Add(tok);
                }

                r = lexer.Report.ToImmutableArray();
            }

            ConcreteSyntaxTree syntaxTree = new(text, ParseTokens);
            report = syntaxTree.Report;
            return tokens.ToImmutableArray();
        }

        internal Node? GetParent(Node Node)
        {
            if (Parents == null)
            {
                Dictionary<Node, Node?>? parents = CreateParentsDictionary(Root);
                Interlocked.CompareExchange(ref Parents, parents, null);
            }

            return Parents[Node];
        }

        private Dictionary<Node, Node?> CreateParentsDictionary(CompilationUnit root)
        {
            Dictionary<Node, Node?>? result = new()
            {
                { root, null }
            };

            CreateParentsDictionary(result, root);
            return result;
        }

        private void CreateParentsDictionary(Dictionary<Node, Node?> result, Node node)
        {
            foreach (Node? child in node.GetChildren())
            {
                result.Add(child, node);
                CreateParentsDictionary(result, child);
            }
        }
    }
}
