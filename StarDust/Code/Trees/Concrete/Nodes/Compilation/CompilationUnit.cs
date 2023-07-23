using System.Collections.Immutable;

namespace StarDust.Code.Syntax
{
    public sealed partial class CompilationUnit : Node
    {
        public override SyntaxType SyntaxType => SyntaxType.COMPILATION_UNIT;
        public ImmutableArray<Member> Members { get; }
        public Token EOF { get; }
        internal CompilationUnit(ConcreteSyntaxTree syntaxTree, ImmutableArray<Member> members, Token eof)
            : base(syntaxTree)
        {
            Members = members;
            EOF = eof;
        }
    }
}
