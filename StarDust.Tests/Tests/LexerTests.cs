using StarDust.Code;
using StarDust.Code.Extensions;
using StarDust.Code.Syntax;
using StarDust.Code.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace StarDust.Test
{
    public class LexerTests
    {
        [Fact]
        public void Lexer_Lexes_UnterminatedString()
        {
            const string source = "\"Hello World";
            System.Collections.Immutable.ImmutableArray<Token> tokens = ConcreteSyntaxTree.ParseTokens(source, out System.Collections.Immutable.ImmutableArray<Report> parseReport);
            Token token = Assert.Single(tokens);
            Assert.Equal(SyntaxType.STRING_TOKEN, token.SyntaxType);
            Assert.Equal(source, token.Text);

            Report? report = Assert.Single(parseReport);
            Assert.Equal(new TextSpan(0, 1), report.Location.Span);
            Assert.Equal("Unterminated string literal.", report.Message);
        }

        [Fact]
        public void Lexer_Tests_AllTokens()
        {
            List<SyntaxType>? tokenTypes = Enum.GetValues(typeof(SyntaxType))
                                    .Cast<SyntaxType>()
                                    .Where(k => k.IsToken())
                                    .ToList();

            IEnumerable<SyntaxType>? testedTokens = GetTokens().Concat(GetSeparators()).Select(t => t.Type);
            SortedSet<SyntaxType> untestedTokens = new(tokenTypes);
            untestedTokens.Remove(SyntaxType.BAD_TOKEN);
            untestedTokens.Remove(SyntaxType.END_OF_FILE_TOKEN);
            untestedTokens.ExceptWith(testedTokens);

            Assert.Empty(untestedTokens);
        }

        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void Lexer_Lexes_Token(SyntaxType Type, string Text)
        {
            System.Collections.Immutable.ImmutableArray<Token> Tokens = ConcreteSyntaxTree.ParseTokens(Text);

            Token? Token = Assert.Single(Tokens);
            Assert.Equal(Type, Token.SyntaxType);
            Assert.Equal(Text, Token.Text);
        }

        [Theory]
        [MemberData(nameof(GetSeparatorsData))]
        public void Lexer_Lexes_Separator(SyntaxType type, string text)
        {
            System.Collections.Immutable.ImmutableArray<Token> tokens = ConcreteSyntaxTree.ParseTokens(text, true);

            Token? token = Assert.Single(tokens);
            Trivia? trivia = Assert.Single(token.LeadingTrivia);

            Assert.Equal(type, trivia.SyntaxType);
            Assert.Equal(text, trivia.Text);
        }

        [Theory]
        [MemberData(nameof(GetTokensPairsData))]
        public void Lexer_Lexes_TokenPairs(SyntaxType Type1, string Text1, SyntaxType Type2, string Text2)
        {
            string? Text = Text1 + Text2;
            Token[]? Tokens = ConcreteSyntaxTree.ParseTokens(Text).ToArray();

            Assert.Equal(2, Tokens.Length);
            Assert.Equal(Type1, Tokens[0].SyntaxType);
            Assert.Equal(Text1, Tokens[0].Text);
            Assert.Equal(Type2, Tokens[1].SyntaxType);
            Assert.Equal(Text2, Tokens[1].Text);
        }

        [Theory]
        [MemberData(nameof(GetTokensPairsWithSeparatorData))]
        public void Lexer_Lexes_TokenPairs_WithSeparator(SyntaxType type1,
                                                         string text1,
                                                         SyntaxType separatorType,
                                                         string separatorText,
                                                         SyntaxType type2,
                                                         string text2)
        {
            string? Text = text1 + separatorText + text2;
            Token[]? Tokens = ConcreteSyntaxTree.ParseTokens(Text).ToArray();

            Assert.Equal(2, Tokens.Length);
            Assert.Equal(type1, Tokens[0].SyntaxType);
            Assert.Equal(text1, Tokens[0].Text);
            // Assert.Equal(SeparatorType, Tokens[1].SyntaxType);
            // Assert.Equal(SeparatorText, Tokens[1].Text);
            Trivia? separator = Assert.Single(Tokens[0].TrailingTrivia);
            Assert.Equal(separatorType, separator.SyntaxType);
            Assert.Equal(separatorText, separator.Text);

            Assert.Equal(type2, Tokens[1].SyntaxType);
            Assert.Equal(text2, Tokens[1].Text);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("foo42")]
        [InlineData("foo_42")]
        [InlineData("_foo")]
        public void Lexer_Lexes_Identifiers(string name)
        {
            Token[]? tokens = ConcreteSyntaxTree.ParseTokens(name).ToArray();

            Assert.Single(tokens);

            Token? token = tokens[0];
            Assert.Equal(SyntaxType.IDENTIFIER_TOKEN, token.SyntaxType);
            Assert.Equal(name, token.Text);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach ((SyntaxType Type, string Text) Token in GetTokens())
            {
                yield return new object[] { Token.Type, Token.Text };
            }
        }

        public static IEnumerable<object[]> GetSeparatorsData()
        {
            foreach ((SyntaxType Type, string Text) Token in GetSeparators())
            {
                yield return new object[] { Token.Type, Token.Text };
            }
        }

        public static IEnumerable<object[]> GetTokensPairsData()
        {
            foreach ((SyntaxType Type1, string Text1, SyntaxType Type2, string Text2) in GetTokensPairs())
            {
                yield return new object[] { Type1, Text1, Type2, Text2 };
            }
        }

        public static IEnumerable<object[]> GetTokensPairsWithSeparatorData()
        {
            foreach ((SyntaxType Type1, string Text1, SyntaxType SeparatorType, string SeparatorText, SyntaxType Type2, string Text2) in GetTokensPairsWithSeparators())
            {
                yield return new object[] { Type1, Text1, SeparatorType, SeparatorText, Type2, Text2 };
            }
        }

        private static bool RequiresSeparator(SyntaxType type1, SyntaxType type2)
        {
            bool t1IsKeyword = type1.IsKeyword();
            bool t2IsKeyword = type2.IsKeyword();

            if (t1IsKeyword && t2IsKeyword)
            {
                return true;
            }

            if (type1 == SyntaxType.STRING_TOKEN && type2 == SyntaxType.STRING_TOKEN)
                return true;

            if (type1 == SyntaxType.NUMBER_TOKEN && type2 == SyntaxType.NUMBER_TOKEN)
                return true;

            if (type1 == SyntaxType.IDENTIFIER_TOKEN && type2 == SyntaxType.IDENTIFIER_TOKEN)
                return true;

            if (type1 == SyntaxType.BANG_TOKEN && type2 == SyntaxType.EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.BANG_TOKEN && type2 == SyntaxType.DOUBLE_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.EQUALS_TOKEN && type2 == SyntaxType.EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.EQUALS_TOKEN && type2 == SyntaxType.DOUBLE_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.LESS_TOKEN && type2 == SyntaxType.EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.LESS_TOKEN && type2 == SyntaxType.DOUBLE_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.GREATER_TOKEN && type2 == SyntaxType.EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.GREATER_TOKEN && type2 == SyntaxType.DOUBLE_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.PLUS_TOKEN && type2 == SyntaxType.EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.PLUS_TOKEN && type2 == SyntaxType.DOUBLE_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.MINUS_TOKEN && type2 == SyntaxType.EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.MINUS_TOKEN && type2 == SyntaxType.DOUBLE_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.STAR_TOKEN && type2 == SyntaxType.EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.STAR_TOKEN && type2 == SyntaxType.DOUBLE_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.SLASH_TOKEN && type2 == SyntaxType.EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.SLASH_TOKEN && type2 == SyntaxType.DOUBLE_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.AMPERSAND_TOKEN && type2 == SyntaxType.DOUBLE_AMPERSAND_TOKEN)
                return true;

            if (type1 == SyntaxType.AMPERSAND_TOKEN && type2 == SyntaxType.AMPERSAND_TOKEN)
                return true;

            if (type1 == SyntaxType.AMPERSAND_TOKEN && type2 == SyntaxType.EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.AMPERSAND_TOKEN && type2 == SyntaxType.DOUBLE_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.AMPERSAND_TOKEN && type2 == SyntaxType.AMPERSAND_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.SLASH_TOKEN && type2 == SyntaxType.SLASH_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.SLASH_TOKEN && type2 == SyntaxType.STAR_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.PIPE_TOKEN && type2 == SyntaxType.DOUBLE_PIPE_TOKEN)
                return true;

            if (type1 == SyntaxType.PIPE_TOKEN && type2 == SyntaxType.PIPE_TOKEN)
                return true;

            if (type1 == SyntaxType.PIPE_TOKEN && type2 == SyntaxType.EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.PIPE_TOKEN && type2 == SyntaxType.DOUBLE_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.PIPE_TOKEN && type2 == SyntaxType.PIPE_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.HAT_TOKEN && type2 == SyntaxType.EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.HAT_TOKEN && type2 == SyntaxType.DOUBLE_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.SLASH_TOKEN && type2 == SyntaxType.SLASH_TOKEN)
                return true;

            if (type1 == SyntaxType.SLASH_TOKEN && type2 == SyntaxType.STAR_TOKEN)
                return true;

            if (type1 == SyntaxType.SLASH_TOKEN && type2 == SyntaxType.SINGLE_LINE_COMMENT_TRIVIA)
                return true;

            if (type1 == SyntaxType.SLASH_TOKEN && type2 == SyntaxType.MULTI_LINE_COMMENT_TRIVIA)
                return true;

            if (type1 == SyntaxType.IDENTIFIER_TOKEN && type2 == SyntaxType.NUMBER_TOKEN)
                return true;

            if (t1IsKeyword && type2 == SyntaxType.NUMBER_TOKEN)
                return true;

            if (t1IsKeyword && type2 == SyntaxType.IDENTIFIER_TOKEN)
                return true;

            if (t2IsKeyword && type1 == SyntaxType.IDENTIFIER_TOKEN)
                return true;

            return false;
        }

        private static IEnumerable<(SyntaxType Type, string Text)> GetTokens()
        {
            IEnumerable<(SyntaxType tokType, string text)>? fixedTokens = Enum.GetValues(typeof(SyntaxType))
                                    .Cast<SyntaxType>()
                                    .Select(k => (tokType: k, text: k.GetText()))
                                    .Where(t => t.text is not null)
                                    .Cast<(SyntaxType tokType, string text)>();

            (SyntaxType, string)[]? dynamicTokens = new[]
            {
                (SyntaxType.IDENTIFIER_TOKEN, "a"),
                (SyntaxType.IDENTIFIER_TOKEN, "abc"),
                (SyntaxType.NUMBER_TOKEN, "1"),
                (SyntaxType.NUMBER_TOKEN, "123"),
                (SyntaxType.STRING_TOKEN, "\"abc\""),
                (SyntaxType.STRING_TOKEN, "\"ab\"\"c\""),
            };

            return fixedTokens.Concat(dynamicTokens);
        }

        private static IEnumerable<(SyntaxType Type, string Text)> GetSeparators() => new[]
            {
                (SyntaxType.WHITESPACE_TRIVIA, " "),
                (SyntaxType.WHITESPACE_TRIVIA, "    "),
                (SyntaxType.LINE_BREAK_TRIVIA, "\r"),
                (SyntaxType.LINE_BREAK_TRIVIA, "\n"),
                (SyntaxType.LINE_BREAK_TRIVIA, "\r\n"),
                (SyntaxType.MULTI_LINE_COMMENT_TRIVIA, "/**/"),
            };

        private static IEnumerable<(SyntaxType Type1, string Text1, SyntaxType Type2, string Text2)> GetTokensPairs()
        {
            foreach ((SyntaxType Type, string Text) Token1 in GetTokens())
            {
                foreach ((SyntaxType Type, string Text) Token2 in GetTokens())
                {
                    if (!RequiresSeparator(Token1.Type, Token2.Type))
                    {
                        yield return (Token1.Type, Token1.Text, Token2.Type, Token2.Text);
                    }
                }
            }
        }

        private static IEnumerable<(SyntaxType Type1, string Text1,
                                    SyntaxType SeparatorType, string SeparatorText,
                                    SyntaxType Type2, string Text2)> GetTokensPairsWithSeparators()
        {
            foreach ((SyntaxType Type, string Text) t1 in GetTokens())
            {
                foreach ((SyntaxType Type, string Text) t2 in GetTokens())
                {
                    if (RequiresSeparator(t1.Type, t2.Type))
                    {
                        foreach ((SyntaxType Type, string Text) s in GetSeparators())
                        {
                            if (!RequiresSeparator(t1.Type, s.Type) && !RequiresSeparator(s.Type, t2.Type))
                                yield return (t1.Type, t1.Text, s.Type, s.Text, t2.Type, t2.Text);
                        }
                    }
                }
            }
        }
    }
}