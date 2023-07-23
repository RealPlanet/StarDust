using StarDust.Code.Extensions;
using StarDust.Code.Symbols;
using StarDust.Code.Text;
using System.Collections.Immutable;
using System.Text;

namespace StarDust.Code.Syntax
{
    internal sealed class Lexer
    {
        #region Properties
        /// <summary> Returns a IEnumerable containing all the errors found during the lexing process. </summary>
        public ReportBag Report { get; } = new();

        /// <summary> Returns the text to be parsed. </summary>
        public SourceText SourceText { get; }
        public ConcreteSyntaxTree ConcreteSyntaxTree { get; }

        #endregion

        #region Private Properties
        private readonly ImmutableArray<Trivia>.Builder TriviaBuilder = ImmutableArray.CreateBuilder<Trivia>();
        private char CurrentCharacter => PeekCharacter(0);
        private char Lookahead => PeekCharacter(1);
        #endregion

        #region Token Specific Data

        struct TokenData
        {
            public int TextPosition, Start;
            public SyntaxType Type;
            public object? Value;

            public TokenData()
            {
                TextPosition = 0;
                Start = 0;
                Type = SyntaxType.BAD_TOKEN;
                Value = null;
            }
        }

        private TokenData _Token;
        #endregion

        public Lexer(ConcreteSyntaxTree syntaxTree)
        {
            SourceText = syntaxTree.Text;
            ConcreteSyntaxTree = syntaxTree;
            _Token = new TokenData();
        }

        /// <summary>
        /// Gennerates a new token from the given source text and returns a new object of type Token
        /// </summary>
        /// <returns>The generated Token from the source text</returns>
        public Token Lex()
        {
            ReadTrivia(leading: true);
            ImmutableArray<Trivia> leadingTrivia = TriviaBuilder.ToImmutable();
            int tokenStart = _Token.TextPosition;

            ReadToken();
            SyntaxType tokenType = _Token.Type;
            object? tokenValue = _Token.Value;
            int tokenLength = _Token.TextPosition - _Token.Start;

            ReadTrivia(leading: false);
            ImmutableArray<Trivia> trailingTrivia = TriviaBuilder.ToImmutable();

            string? tokenText = tokenType.GetText() ?? SourceText.ToString(tokenStart, tokenLength);

            return new Token(ConcreteSyntaxTree, tokenType, tokenStart, tokenText, tokenValue, leadingTrivia, trailingTrivia);
        }

        /// <summary>
        /// Reads trivia from the source text
        /// </summary>
        private void ReadTrivia(bool leading)
        {
            TriviaBuilder.Clear();
            bool isDone = false;

            while (!isDone)
            {
                _Token.Start = _Token.TextPosition;
                _Token.Type = SyntaxType.BAD_TOKEN;
                _Token.Value = null;

                switch (CurrentCharacter)
                {
                    case '\0':
                        isDone = true;
                        break;
                    case '/':
                        {
                            if (Lookahead == '/')
                            {
                                ReadSingleLineComment();
                                break;
                            }

                            if (Lookahead == '*')
                            {
                                ReadMultiLineComment();
                                break;
                            }
                            isDone = true;
                            break;
                        }
                    case '\r':
                    case '\n':
                        {
                            // if the trivia is trailing then it stops at the first line break or whitespace
                            if (!leading)
                                isDone = true;

                            ReadLineBreak();
                            break;
                        }
                    default:
                        // This function is a more expensive check for whitespace if the four most common cases don't occour.
                        if (char.IsWhiteSpace(CurrentCharacter))
                        {
                            ReadWhiteSpace();
                            break;
                        }

                        isDone = true;
                        break;
                }
                int triviaLength = _Token.TextPosition - _Token.Start;
                if (triviaLength > 0)
                {
                    string text = SourceText.ToString(_Token.Start, triviaLength);
                    Trivia trivia = new(ConcreteSyntaxTree, _Token.Type, _Token.Start, text);
                    TriviaBuilder.Add(trivia);
                }
            }
        }
        private void ReadWhiteSpace()
        {
            bool done = false;
            while (!done)
            {
                switch (CurrentCharacter)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        done = true;
                        break;
                    default:
                        if (!char.IsWhiteSpace(CurrentCharacter))
                            done = true;
                        else
                            _Token.TextPosition++;
                        break;
                }
            }

            _Token.Type = SyntaxType.WHITESPACE_TRIVIA;
        }

        private void ReadLineBreak()
        {
            if (CurrentCharacter == '\r' && Lookahead == '\n')
            {
                _Token.TextPosition += 2;
            }
            else
            {
                _Token.TextPosition++;
            }

            _Token.Type = SyntaxType.LINE_BREAK_TRIVIA;
        }

        /// <summary cref="Token">
        /// Reads a new token from the given source text
        /// </summary>
        private void ReadToken()
        {
            _Token.Start = _Token.TextPosition;
            _Token.Type = SyntaxType.BAD_TOKEN;
            _Token.Value = null;

            switch (CurrentCharacter)
            {
                case '\0':
                    {
                        _Token.Type = SyntaxType.END_OF_FILE_TOKEN;
                        break;
                    }
                case ';':
                    {
                        _Token.Type = SyntaxType.SEMICOLON_TOKEN;
                        _Token.TextPosition++;
                        break;
                    }
                case '+':
                    {
                        _Token.Type = SyntaxType.PLUS_TOKEN;
                        _Token.TextPosition++;
                        if (CurrentCharacter == '=')
                        {
                            _Token.Type = SyntaxType.PLUS_EQUALS_TOKEN;
                            _Token.TextPosition++;
                        }
                        break;
                    }
                case '-':
                    {
                        _Token.Type = SyntaxType.MINUS_TOKEN;
                        _Token.TextPosition++;
                        if (CurrentCharacter == '=')
                        {
                            _Token.Type = SyntaxType.MINUS_EQUALS_TOKEN;
                            _Token.TextPosition++;
                        }
                        break;
                    }
                case '*':
                    {
                        _Token.Type = SyntaxType.STAR_TOKEN;
                        _Token.TextPosition++;
                        if (CurrentCharacter == '=')
                        {
                            _Token.Type = SyntaxType.STAR_EQUALS_TOKEN;
                            _Token.TextPosition++;
                        }
                        break;
                    }
                case '/':
                    {
                        _Token.Type = SyntaxType.SLASH_TOKEN;
                        _Token.TextPosition++;
                        if (CurrentCharacter == '=')
                        {
                            _Token.Type = SyntaxType.SLASH_EQUALS_TOKEN;
                            _Token.TextPosition++;
                        }
                        break;
                    }
                case '!':
                    {
                        _Token.Type = SyntaxType.BANG_TOKEN;
                        _Token.TextPosition++;
                        if (CurrentCharacter == '=')
                        {
                            _Token.Type = SyntaxType.BANG_EQUALS_TOKEN;
                            _Token.TextPosition++;
                            break;
                        }

                        break;
                    }
                case '=':
                    {
                        _Token.Type = SyntaxType.EQUALS_TOKEN;
                        _Token.TextPosition++;

                        if (CurrentCharacter == '=')
                        {
                            _Token.Type = SyntaxType.DOUBLE_EQUALS_TOKEN;
                            _Token.TextPosition++;
                            break;
                        }

                        break;
                    }
                case '&':
                    {
                        _Token.Type = SyntaxType.AMPERSAND_TOKEN;
                        _Token.TextPosition++;
                        if (CurrentCharacter == '&')
                        {
                            _Token.Type = SyntaxType.DOUBLE_AMPERSAND_TOKEN;
                            _Token.TextPosition++;
                            break;
                        }

                        if (CurrentCharacter == '=')
                        {
                            _Token.Type = SyntaxType.AMPERSAND_EQUALS_TOKEN;
                            _Token.TextPosition++;
                        }
                        break;
                    }
                case '|':
                    {
                        _Token.Type = SyntaxType.PIPE_TOKEN;
                        _Token.TextPosition++;
                        if (CurrentCharacter == '|')
                        {
                            _Token.Type = SyntaxType.DOUBLE_PIPE_TOKEN;
                            _Token.TextPosition++;
                            break;
                        }

                        if (CurrentCharacter == '=')
                        {
                            _Token.Type = SyntaxType.PIPE_EQUALS_TOKEN;
                            _Token.TextPosition++;
                        }
                        break;
                    }
                case '<':
                    {
                        _Token.Type = SyntaxType.LESS_TOKEN;
                        _Token.TextPosition++;
                        if (CurrentCharacter == '=')
                        {
                            _Token.Type = SyntaxType.LESS_OR_EQUALS_TOKEN;
                            _Token.TextPosition++;
                            break;
                        }

                        break;
                    }
                case '>':
                    {
                        _Token.Type = SyntaxType.GREATER_TOKEN;
                        _Token.TextPosition++;
                        if (CurrentCharacter == '=')
                        {
                            _Token.Type = SyntaxType.GREATER_EQUAL_TOKEN;
                            _Token.TextPosition++;
                            break;
                        }
                        break;
                    }
                case '(':
                    {
                        _Token.Type = SyntaxType.OPEN_PARENTHESIS_TOKEN;
                        _Token.TextPosition++;
                        break;
                    }
                case ')':
                    {
                        _Token.Type = SyntaxType.CLOSE_PARENTHESIS_TOKEN;
                        _Token.TextPosition++;
                        break;
                    }
                case '{':
                    {
                        _Token.Type = SyntaxType.OPEN_BRACE_TOKEN;
                        _Token.TextPosition++;
                        break;
                    }
                case '}':
                    {
                        _Token.Type = SyntaxType.CLOSE_BRACE_TOKEN;
                        _Token.TextPosition++;
                        break;
                    }
                case ':':
                    {
                        _Token.Type = SyntaxType.COLON_TOKEN;
                        _Token.TextPosition++;
                        break;
                    }
                case ',':
                    {
                        _Token.Type = SyntaxType.COMMA_TOKEN;
                        _Token.TextPosition++;
                        break;
                    }
                case '~':
                    {
                        _Token.Type = SyntaxType.TILDE_TOKEN;
                        _Token.TextPosition++;
                        break;
                    }
                case '^':
                    {
                        _Token.Type = SyntaxType.HAT_TOKEN;
                        _Token.TextPosition++;
                        if (CurrentCharacter == '=')
                        {
                            _Token.Type = SyntaxType.HAT_EQUALS_TOKEN;
                            _Token.TextPosition++;
                        }
                        break;
                    }
                case '\"':
                    {
                        ReadString();
                        break;
                    }
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    {
                        ReadNumberToken();
                        break;
                    }
                case '_':
                    ReadIdentifierOrKeyword();
                    break;
                default:
                    {
                        if (char.IsLetter(CurrentCharacter))
                        {
                            ReadIdentifierOrKeyword();
                        }
                        else
                        {
                            TextSpan span = new(_Token.TextPosition, 1);
                            TextLocation location = new(SourceText, span);
                            Report.ReportBadCharacter(location, CurrentCharacter);
                            _Token.TextPosition++;
                        }
                        break;
                    }
            }
        }

        private void ReadSingleLineComment()
        {
            // Skip leading slash
            _Token.TextPosition += 2;
            bool done = false;

            while (!done)
            {
                switch (CurrentCharacter)
                {
                    case '\r':
                    case '\n':
                    case '\0':
                        done = true;
                        break;
                    default:
                        _Token.TextPosition++;
                        break;
                }
            }

            _Token.Type = SyntaxType.SINGLE_LINE_COMMENT_TRIVIA;
        }

        private void ReadMultiLineComment()
        {
            // Skip leading slash
            _Token.TextPosition += 2;
            bool done = false;

            while (!done)
            {
                switch (CurrentCharacter)
                {
                    case '\0':
                        TextSpan span = new(_Token.Start, 2);
                        TextLocation location = new(SourceText, span);
                        Report.ReportUnterminatedMultiLineComment(location);
                        done = true;
                        break;
                    case '*':
                        if (Lookahead == '/')
                        {
                            done = true;
                            _Token.TextPosition++;
                        }
                        _Token.TextPosition++;
                        break;
                    default:
                        _Token.TextPosition++;
                        break;
                }
            }

            _Token.Type = SyntaxType.MULTI_LINE_COMMENT_TRIVIA;
        }

        private void ReadString()
        {
            // "Test "" other" --> "" escapes into "
            // Value becomes --> Test " other

            _Token.TextPosition++; // Skip initial "

            StringBuilder sb = new();
            bool done = false;

            void consumeChar(char _)
            {
                sb.Append(CurrentCharacter);
                _Token.TextPosition++;
            }

            while (!done)
            {
                switch (CurrentCharacter)
                {
                    case '"':
                        {
                            if (Lookahead == '"')
                            {
                                consumeChar(CurrentCharacter);
                                _Token.TextPosition++;
                                break;
                            }

                            _Token.TextPosition++;
                            done = true;
                            break;
                        }
                    case '\0':
                    case '\r':
                    case '\n':
                        TextSpan span = new(_Token.Start, 1);
                        TextLocation location = new(SourceText, span);
                        Report.ReportUnterminatedString(location);
                        done = true;
                        break;
                    default:
                        consumeChar(CurrentCharacter);
                        break;
                }
            }

            _Token.Type = SyntaxType.STRING_TOKEN;
            _Token.Value = sb.ToString();
        }

        private void ReadIdentifierOrKeyword()
        {
            while (char.IsLetterOrDigit(CurrentCharacter) || CurrentCharacter == '_')
            {
                _Token.TextPosition++;
            }

            string Text = SourceText.ToString(_Token.Start, _Token.TextPosition - _Token.Start);
            _Token.Type = Text.GetKeywordType();
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(CurrentCharacter))
            {
                _Token.TextPosition++;
            }

            int len = _Token.TextPosition - _Token.Start;
            string text = SourceText.ToString(_Token.Start, len);
            if (!int.TryParse(text, out int Value))
            {
                TextSpan span = new(_Token.Start, len);
                TextLocation location = new(SourceText, span);
                Report.ReportInvalidNumber(location, text, TypeSymbol.Int);
            }

            _Token.Value = Value;
            _Token.Type = SyntaxType.NUMBER_TOKEN;
        }

        private char PeekCharacter(int Offset)
        {
            int Position = _Token.TextPosition + Offset;
            if (Position >= SourceText.Length)
            {
                return '\0';
            }
            return SourceText[Position];
        }
    }
}
