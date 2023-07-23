using StarDust.Code.Syntax;

namespace StarDust.Code.Extensions
{
    public static class SyntaxExtensions
    {
        #region General
        public static IEnumerable<SyntaxType> GetBinaryOperatorTypes()
        {
            SyntaxType[]? Types = (SyntaxType[])Enum.GetValues(typeof(SyntaxType));
            foreach (SyntaxType Type in Types)
            {
                if (GetBinaryOperatorPrecedence(Type) > 0)
                {
                    yield return Type;
                }
            }
        }
        public static IEnumerable<SyntaxType> GetUnaryOperatorTypes()
        {
            SyntaxType[]? Types = (SyntaxType[])Enum.GetValues(typeof(SyntaxType));
            foreach (SyntaxType Type in Types)
            {
                if (GetUnaryOperatorPrecedence(Type) > 0)
                {
                    yield return Type;
                }
            }
        }

        #endregion

        public static int GetBinaryOperatorPrecedence(this SyntaxType Type)
        {
            return Type switch
            {
                SyntaxType.STAR_TOKEN or
                SyntaxType.SLASH_TOKEN => 5,
                SyntaxType.PLUS_TOKEN or
                SyntaxType.MINUS_TOKEN => 4,
                SyntaxType.BANG_EQUALS_TOKEN or
                SyntaxType.DOUBLE_EQUALS_TOKEN or
                SyntaxType.LESS_TOKEN or
                SyntaxType.LESS_OR_EQUALS_TOKEN or
                SyntaxType.GREATER_TOKEN or
                SyntaxType.GREATER_EQUAL_TOKEN => 3,

                // AND
                SyntaxType.AMPERSAND_TOKEN or
                SyntaxType.DOUBLE_AMPERSAND_TOKEN => 2,
                // OR
                SyntaxType.HAT_TOKEN or
                SyntaxType.PIPE_TOKEN or
                SyntaxType.DOUBLE_PIPE_TOKEN => 1,
                _ => 0,
            };
        }

        public static int GetUnaryOperatorPrecedence(this SyntaxType Type)
        {
            return Type switch
            {
                SyntaxType.PLUS_TOKEN or
                SyntaxType.MINUS_TOKEN or
                SyntaxType.BANG_TOKEN or
                SyntaxType.TILDE_TOKEN or
                SyntaxType.HAT_TOKEN => 6,
                _ => 0,
            };
        }

        public static SyntaxType GetKeywordType(this string text)
        {
            return text switch
            {
                "break" => SyntaxType.BREAK_KEYWORD,
                "continue" => SyntaxType.CONTINUE_KEYWORD,
                "if" => SyntaxType.IF_KEYWORD,
                "else" => SyntaxType.ELSE_KEYWORD,
                "true" => SyntaxType.TRUE_KEYWORD,
                "false" => SyntaxType.FALSE_KEYWORD,
                "const" => SyntaxType.CONST_KEYWORD,
                "var" => SyntaxType.VAR_KEYWORD,
                "while" => SyntaxType.WHILE_KEYWORD,
                "do" => SyntaxType.DO_KEYWORD,
                "for" => SyntaxType.FOR_KEYWORD,
                "to" => SyntaxType.TO_KEYWORD,
                "fn" => SyntaxType.FUNCTION_KEYWORD,
                "return" => SyntaxType.RETURN_KEYWORD,
                _ => SyntaxType.IDENTIFIER_TOKEN,
            };
        }

        public static string? GetText(this SyntaxType kind)
        {
            return kind switch
            {
                SyntaxType.PLUS_TOKEN => "+",
                SyntaxType.PLUS_EQUALS_TOKEN => "+=",
                SyntaxType.MINUS_TOKEN => "-",
                SyntaxType.MINUS_EQUALS_TOKEN => "-=",
                SyntaxType.STAR_TOKEN => "*",
                SyntaxType.STAR_EQUALS_TOKEN => "*=",
                SyntaxType.SLASH_TOKEN => "/",
                SyntaxType.SLASH_EQUALS_TOKEN => "/=",
                SyntaxType.BANG_TOKEN => "!",
                SyntaxType.EQUALS_TOKEN => "=",
                SyntaxType.AMPERSAND_TOKEN => "&",
                SyntaxType.AMPERSAND_EQUALS_TOKEN => "&=",
                SyntaxType.DOUBLE_AMPERSAND_TOKEN => "&&",
                SyntaxType.PIPE_TOKEN => "|",
                SyntaxType.PIPE_EQUALS_TOKEN => "|=",
                SyntaxType.DOUBLE_PIPE_TOKEN => "||",
                SyntaxType.HAT_TOKEN => "^",
                SyntaxType.HAT_EQUALS_TOKEN => "^=",
                SyntaxType.TILDE_TOKEN => "~",
                SyntaxType.DOUBLE_EQUALS_TOKEN => "==",
                SyntaxType.BANG_EQUALS_TOKEN => "!=",
                SyntaxType.LESS_TOKEN => "<",
                SyntaxType.LESS_OR_EQUALS_TOKEN => "<=",
                SyntaxType.GREATER_TOKEN => ">",
                SyntaxType.GREATER_EQUAL_TOKEN => ">=",
                SyntaxType.OPEN_PARENTHESIS_TOKEN => "(",
                SyntaxType.CLOSE_PARENTHESIS_TOKEN => ")",
                SyntaxType.OPEN_BRACE_TOKEN => "{",
                SyntaxType.CLOSE_BRACE_TOKEN => "}",
                SyntaxType.COLON_TOKEN => ":",
                SyntaxType.COMMA_TOKEN => ",",
                SyntaxType.FALSE_KEYWORD => "false",
                SyntaxType.TRUE_KEYWORD => "true",
                SyntaxType.VAR_KEYWORD => "var",
                SyntaxType.CONST_KEYWORD => "const",
                SyntaxType.BREAK_KEYWORD => "break",
                SyntaxType.CONTINUE_KEYWORD => "continue",
                SyntaxType.IF_KEYWORD => "if",
                SyntaxType.ELSE_KEYWORD => "else",
                SyntaxType.WHILE_KEYWORD => "while",
                SyntaxType.DO_KEYWORD => "do",
                SyntaxType.FOR_KEYWORD => "for",
                SyntaxType.TO_KEYWORD => "to",
                SyntaxType.FUNCTION_KEYWORD => "fn",
                SyntaxType.RETURN_KEYWORD => "return",
                _ => null,
            };
        }

        public static SyntaxType GetBinaryOperatorOfAssignmentOperator(this SyntaxType kind)
        {
            return kind switch
            {
                SyntaxType.PLUS_EQUALS_TOKEN => SyntaxType.PLUS_TOKEN,
                SyntaxType.MINUS_EQUALS_TOKEN => SyntaxType.MINUS_TOKEN,
                SyntaxType.STAR_EQUALS_TOKEN => SyntaxType.STAR_TOKEN,
                SyntaxType.SLASH_EQUALS_TOKEN => SyntaxType.SLASH_TOKEN,
                SyntaxType.AMPERSAND_EQUALS_TOKEN => SyntaxType.AMPERSAND_TOKEN,
                SyntaxType.PIPE_EQUALS_TOKEN => SyntaxType.PIPE_TOKEN,
                SyntaxType.HAT_EQUALS_TOKEN => SyntaxType.HAT_TOKEN,
                _ => throw new Exception($"Invalid syntax kind: '{kind}'"),
            };
        }

        public static bool IsKeyword(this SyntaxType type)
        {
            return type.ToString().EndsWith("keyword", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsToken(this SyntaxType type)
        {
            return !type.IsTrivia() && (type.IsKeyword() || type.ToString().EndsWith("token", StringComparison.CurrentCultureIgnoreCase));
        }

        public static bool IsComment(this SyntaxType type)
        {
            return type is SyntaxType.SINGLE_LINE_COMMENT_TRIVIA or SyntaxType.MULTI_LINE_COMMENT_TRIVIA;
        }

        public static bool IsTrivia(this SyntaxType type)
        {
            return type switch
            {
                SyntaxType.MULTI_LINE_COMMENT_TRIVIA or SyntaxType.SINGLE_LINE_COMMENT_TRIVIA or
                SyntaxType.WHITESPACE_TRIVIA or SyntaxType.LINE_BREAK_TRIVIA or
                SyntaxType.SKIPPED_TEXT_TRIVIA => true,
                _ => false,
            };
        }
    }
}
