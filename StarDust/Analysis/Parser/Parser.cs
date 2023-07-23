using StarDust.Code.Extensions;
using StarDust.Code.Text;
using System.Collections.Immutable;

namespace StarDust.Code.Syntax
{
    internal sealed class Parser
    {
        #region Data
        private readonly ImmutableArray<Token> TokenToParse;
        private int Position;
        public ReportBag Report { get; } = new();
        private Token Current => Peek(0);
        public ConcreteSyntaxTree SyntaxTree { get; }
        public SourceText Text { get; }
        #endregion

        public Parser(ConcreteSyntaxTree syntaxTree)
        {
            List<Token> tokens = new();
            List<Token> badTokens = new();

            Lexer lexer = new(syntaxTree);
            Token currentToken;
            do
            {
                currentToken = lexer.Lex();

                if (currentToken.SyntaxType == SyntaxType.BAD_TOKEN)
                {
                    badTokens.Add(currentToken);
                    continue;
                }

                if (badTokens.Count > 0)
                {
                    ImmutableArray<Trivia>.Builder leadingTrivia = currentToken.LeadingTrivia.ToBuilder();
                    int index = 0;
                    foreach (Token? token in badTokens)
                    {
                        foreach (Trivia? lt in token.LeadingTrivia)
                            leadingTrivia.Insert(index++, lt);

                        Trivia trivia = new(currentToken.SyntaxTree, SyntaxType.SKIPPED_TEXT_TRIVIA, token.TextPosition, token.Text);
                        leadingTrivia.Insert(index++, trivia);

                        foreach (Trivia? tt in token.TrailingTrivia)
                            leadingTrivia.Insert(index++, tt);
                    }

                    badTokens.Clear();
                    currentToken = new(
                        currentToken.SyntaxTree,
                        currentToken.SyntaxType,
                        currentToken.TextPosition,
                        currentToken.Text,
                        currentToken.Value,
                        leadingTrivia.ToImmutable(),
                        currentToken.TrailingTrivia);
                }

                tokens.Add(currentToken);
            } while (currentToken.SyntaxType != SyntaxType.END_OF_FILE_TOKEN);
            SyntaxTree = syntaxTree;
            Text = syntaxTree.Text;
            TokenToParse = tokens.ToImmutableArray();
            Report.AddRange(lexer.Report);
        }

        public CompilationUnit ParseCompilationUnit()
        {
            ImmutableArray<Member> members = ParseMembers();
            Token EOF = MatchToken(SyntaxType.END_OF_FILE_TOKEN);
            return new CompilationUnit(SyntaxTree, members, EOF);
        }

        private Statement ParseStatement()
        {
            return Current.SyntaxType switch
            {
                SyntaxType.OPEN_BRACE_TOKEN => ParseBlockStatement(),
                SyntaxType.CONST_KEYWORD => ParseVariableDeclaration(),
                SyntaxType.VAR_KEYWORD => ParseVariableDeclaration(),
                SyntaxType.IF_KEYWORD => ParseIfStatement(),
                SyntaxType.WHILE_KEYWORD => ParseWhileStatement(),
                SyntaxType.DO_KEYWORD => ParseDoWhileStatement(),
                SyntaxType.FOR_KEYWORD => ParseForStatement(),
                SyntaxType.BREAK_KEYWORD => ParseBreakStatement(),
                SyntaxType.CONTINUE_KEYWORD => ParseContinueStatement(),
                SyntaxType.RETURN_KEYWORD => ParseReturnStatement(),
                _ => ParseExpressionStatement(),
            };
        }

        private ImmutableArray<Member> ParseMembers()
        {
            ImmutableArray<Member>.Builder? members = ImmutableArray.CreateBuilder<Member>();

            while (Current.SyntaxType is not SyntaxType.END_OF_FILE_TOKEN)
            {
                Token startToken = Current;

                Member member = ParseMember();
                members.Add(member);

                // If ParseStatement did not consume any tokens, we need to skip the current token and continue
                // in order to avoid an infinite loop.
                // We do not need to report an error, we already tried to
                // parse an expression statement and reported one
                if (Current == startToken)
                {
                    NextToken();
                }
            }

            return members.ToImmutable();
        }

        private Member ParseMember()
        {
            if (Current.SyntaxType == SyntaxType.FUNCTION_KEYWORD)
                return ParseFunctionDeclaration();

            return ParseGlobalStatement();
        }

        private Parameter ParseParameter()
        {
            TypeClause type = ParseTypeClause();
            Token identifier = MatchToken(SyntaxType.IDENTIFIER_TOKEN);
            return new Parameter(SyntaxTree, type, identifier);
        }

        private Member ParseFunctionDeclaration()
        {
            Token functionKeyword = MatchToken(SyntaxType.FUNCTION_KEYWORD);
            TypeClause? type = ParseOptionalTypeClause();
            Token identifier = MatchToken(SyntaxType.IDENTIFIER_TOKEN);
            Token openParenthesis = MatchToken(SyntaxType.OPEN_PARENTHESIS_TOKEN);
            SeparatedSyntaxList<Parameter>? expression = ParseParameterList();
            Token closeParenthesis = MatchToken(SyntaxType.CLOSE_PARENTHESIS_TOKEN);
            Statement blockStatement = ParseBlockStatement();
            return new FunctionDeclaration(SyntaxTree, functionKeyword, type, identifier, openParenthesis, expression, closeParenthesis, blockStatement);
        }

        #region Root Expressions
        private Member ParseGlobalStatement()
        {
            Statement statement = ParseStatement();
            return new GlobalStatement(SyntaxTree, statement);
        }

        private Expression ParsePrimaryExpression()
        {
            return Current.SyntaxType switch
            {
                SyntaxType.OPEN_PARENTHESIS_TOKEN => ParseParenthesizedExpression(),
                SyntaxType.TRUE_KEYWORD or SyntaxType.FALSE_KEYWORD => ParseBooleanLiteral(),
                SyntaxType.NUMBER_TOKEN => ParseNumberLiteral(),
                SyntaxType.STRING_TOKEN => ParseStringLiteral(),
                _ => ParseNameOrCallEpression(),
            };
        }

        #endregion

        #region Loops
        private Statement ParseDoWhileStatement()
        {
            Token doKeyword = MatchToken(SyntaxType.DO_KEYWORD);
            Statement body = ParseStatement();
            Token whileKeyword = MatchToken(SyntaxType.WHILE_KEYWORD);

            ParenthesisExpression condition = (ParenthesisExpression)ParseParenthesizedExpression();
            Token sc = MatchToken(SyntaxType.SEMICOLON_TOKEN); // end-of-statement / expression

            // FIXME replace condition tokens with a single ParenthesisExpression
            return new DoWhileStatement(SyntaxTree,
                                        doKeyword,
                                        body,
                                        whileKeyword,
                                        condition.OpenParenthesisToken,
                                        condition.Expression,
                                        condition.ClosedParenthesisToken);
        }

        private Statement ParseForStatement()
        {
            Token keyword = MatchToken(SyntaxType.FOR_KEYWORD);
            Token identifier = MatchToken(SyntaxType.IDENTIFIER_TOKEN);
            Token equals = MatchToken(SyntaxType.EQUALS_TOKEN);
            Expression lowerBound = ParseExpression();
            Token toToken = MatchToken(SyntaxType.TO_KEYWORD);
            Expression upperBound = ParseExpression();
            Statement body = ParseStatement();
            return new ForStatement(SyntaxTree, keyword, identifier, equals, lowerBound, toToken, upperBound, body);
        }

        private Statement ParseWhileStatement()
        {
            Token keyword = MatchToken(SyntaxType.WHILE_KEYWORD);
            ParenthesisExpression condition = (ParenthesisExpression)ParseParenthesizedExpression();
            Statement body = ParseStatement();
            return new WhileStatement(SyntaxTree,
                                      keyword,
                                      condition.OpenParenthesisToken,
                                      condition.Expression,
                                      condition.ClosedParenthesisToken,
                                      body);
        }
        #endregion

        #region IfElse
        private Statement ParseIfStatement()
        {
            Token keyword = MatchToken(SyntaxType.IF_KEYWORD);
            Token openP = MatchToken(SyntaxType.OPEN_PARENTHESIS_TOKEN);
            Expression condition = ParseExpression();
            Token closeP = MatchToken(SyntaxType.CLOSE_PARENTHESIS_TOKEN);

            Statement statement = ParseStatement();
            ElseClauseStatement? elseClause = ParseOptionalElseClause();

            return new IfStatement(SyntaxTree, keyword, openP, condition, closeP, statement, elseClause);
        }

        private ElseClauseStatement? ParseOptionalElseClause()
        {
            if (Current.SyntaxType is not SyntaxType.ELSE_KEYWORD)
                return null;

            Token keyword = MatchToken(SyntaxType.ELSE_KEYWORD);
            Statement statement = ParseStatement();

            return new(SyntaxTree, keyword, statement);
        }
        #endregion

        #region Variables
        private Statement ParseVariableDeclaration()
        {
            // Currently a variable is defined like so:
            //
            //  (var | const) [type] name = value
            //
            SyntaxType expected = Current.SyntaxType == SyntaxType.CONST_KEYWORD ?
                SyntaxType.CONST_KEYWORD : SyntaxType.VAR_KEYWORD;

            Token keyword = MatchToken(expected);
            TypeClause? typeClause = ParseOptionalTypeClause();
            Token identifier = MatchToken(SyntaxType.IDENTIFIER_TOKEN);
            Token equals = MatchToken(SyntaxType.EQUALS_TOKEN);
            Expression initializer = ParseExpression();
            Token sc = MatchToken(SyntaxType.SEMICOLON_TOKEN); // end-of-statement / expression

            return new VariableDeclaration(SyntaxTree, keyword, typeClause, identifier, equals, initializer);
        }

        #endregion

        #region Type
        private TypeClause? ParseOptionalTypeClause()
        {
            bool typeFollowedByIdentifierName = Current.SyntaxType == SyntaxType.IDENTIFIER_TOKEN &&
                Peek(1).SyntaxType == SyntaxType.IDENTIFIER_TOKEN;

            if (!typeFollowedByIdentifierName)
                return null;

            return ParseTypeClause();
        }

        private TypeClause ParseTypeClause()
        {
            Token identifier = MatchToken(SyntaxType.IDENTIFIER_TOKEN);

            return new TypeClause(SyntaxTree, identifier);
        }
        #endregion

        private Statement ParseExpressionStatement()
        {
            Expression expression = ParseExpression();
            return new ExpressionStatement(SyntaxTree, expression);
        }

        private Statement ParseBlockStatement()
        {
            ImmutableArray<Statement>.Builder? statements = ImmutableArray.CreateBuilder<Statement>();

            Token openBraceToken = MatchToken(SyntaxType.OPEN_BRACE_TOKEN);

            while (Current.SyntaxType is not SyntaxType.END_OF_FILE_TOKEN && Current.SyntaxType is not SyntaxType.CLOSE_BRACE_TOKEN)
            {
                Token startToken = Current;

                Statement statement = ParseStatement();
                statements.Add(statement);

                // If ParseStatement did not consume any tokens, we need to skip the current token and continue
                // in order to avoid an infinite loop.
                // We do not need to report an error, we already tried to
                // parse an expression statement and reported one
                if (Current == startToken)
                {
                    NextToken();
                }
            }

            Token closeBraceToken = MatchToken(SyntaxType.CLOSE_BRACE_TOKEN);
            return new BlockStatement(SyntaxTree, openBraceToken, statements.ToImmutable(), closeBraceToken);
        }

        private Token Peek(int Offset)
        {
            if (Position + Offset >= TokenToParse.Length)
            {
                return TokenToParse[^1]; // size -1
            }

            return TokenToParse[Position + Offset];
        }

        private Token NextToken()
        {
            Token current = Current;
            Position++;
            return current;
        }

        /// <summary>
        /// If the current token is of the given type, return the next one. Otherwise, return a generated token of the required type.<br/>
        /// The generated token will have the same position as the current token. It will only be used for the expected syntax tree and
        /// a error will be generated.
        /// </summary>
        /// <param name="Type"> The type to match </param>
        /// <returns> The next token or a generated one</returns>
        private Token MatchToken(SyntaxType Type)
        {
            if (Current.SyntaxType == Type)
            {
                return NextToken();
            }

            Report.ReportUnexpectedToken(Current.Location, Current.SyntaxType, Type);
            return new(SyntaxTree, Type, Current.TextPosition, null, null, ImmutableArray<Trivia>.Empty, ImmutableArray<Trivia>.Empty);
        }

        private Expression ParseExpression()
        {
            return ParseAssignmentExpression();
        }
        #region Control Flow
        private Statement ParseReturnStatement()
        {
            // return 12
            // return <void>

            Token keyword = MatchToken(SyntaxType.RETURN_KEYWORD);
            Expression? expression = null;
            if (Current.SyntaxType != SyntaxType.SEMICOLON_TOKEN)
                expression = ParseExpression();

            Token sc = MatchToken(SyntaxType.SEMICOLON_TOKEN);
            return new ReturnStatement(SyntaxTree, keyword, expression);
        }
        #endregion


        private Statement ParseContinueStatement()
        {
            Token keyword = MatchToken(SyntaxType.CONTINUE_KEYWORD);
            Token sc = MatchToken(SyntaxType.SEMICOLON_TOKEN); // end-of-statement / expression

            return new ContinueStatement(SyntaxTree, keyword);
        }

        private Statement ParseBreakStatement()
        {
            Token keyword = MatchToken(SyntaxType.BREAK_KEYWORD);
            Token sc = MatchToken(SyntaxType.SEMICOLON_TOKEN); // end-of-statement / expression

            return new BreakStatement(SyntaxTree, keyword);
        }

        private Expression ParseAssignmentExpression()
        {
            if (Current.SyntaxType == SyntaxType.IDENTIFIER_TOKEN)
            {
                switch (Peek(1).SyntaxType)
                {
                    case SyntaxType.PLUS_EQUALS_TOKEN:
                    case SyntaxType.MINUS_EQUALS_TOKEN:
                    case SyntaxType.STAR_EQUALS_TOKEN:
                    case SyntaxType.SLASH_EQUALS_TOKEN:
                    case SyntaxType.AMPERSAND_EQUALS_TOKEN:
                    case SyntaxType.PIPE_EQUALS_TOKEN:
                    case SyntaxType.HAT_EQUALS_TOKEN:
                    case SyntaxType.EQUALS_TOKEN:
                        Token identifierToken = NextToken();
                        Token operatorToken = NextToken();
                        Expression rightExpression = ParseAssignmentExpression();
                        Token semiColon = MatchToken(SyntaxType.SEMICOLON_TOKEN);

                        return new AssignmentExpression(SyntaxTree, identifierToken, operatorToken, rightExpression);
                }
            }

            return ParseBinaryExpression();
        }

        private Expression ParseBinaryExpression(int ParentPrecedence = 0)
        {
            Expression Left;
            int UnaryPrecedence = Current.SyntaxType.GetUnaryOperatorPrecedence();
            if (UnaryPrecedence != 0 && UnaryPrecedence >= ParentPrecedence)
            {
                Token? OperatorToken = NextToken();
                Expression? Operand = ParseBinaryExpression(UnaryPrecedence);
                Left = new UnaryExpression(SyntaxTree, OperatorToken, Operand);
            }
            else
            {
                Left = ParsePrimaryExpression();
            }

            while (true)
            {
                int Precedence = Current.SyntaxType.GetBinaryOperatorPrecedence();
                if (Precedence == 0 || Precedence <= ParentPrecedence)
                {
                    break;
                }

                Token? OperatorToken = NextToken();
                Expression? Right = ParseBinaryExpression(Precedence);
                Left = new BinaryExpression(SyntaxTree, Left, OperatorToken, Right);
            }

            return Left;
        }

        #region Literals
        private Expression ParseNumberLiteral()
        {
            Token NumberToken = MatchToken(SyntaxType.NUMBER_TOKEN);
            return new LiteralExpression(SyntaxTree, NumberToken);
        }

        private Expression ParseBooleanLiteral()
        {
            bool isTrue = Current.SyntaxType == SyntaxType.TRUE_KEYWORD;
            Token boolean = isTrue ? MatchToken(SyntaxType.TRUE_KEYWORD)
                : MatchToken(SyntaxType.FALSE_KEYWORD);

            //Token sc = MatchToken(SyntaxType.SEMICOLON_TOKEN); // end-of-statement / expression

            return new LiteralExpression(SyntaxTree, boolean, isTrue);
        }

        private Expression ParseStringLiteral()
        {
            Token stringToken = MatchToken(SyntaxType.STRING_TOKEN);
            //Token sc = MatchToken(SyntaxType.SEMICOLON_TOKEN); // end-of-statement / expression

            return new LiteralExpression(SyntaxTree, stringToken);
        }

        #endregion

        private Expression ParseParenthesizedExpression()
        {
            Token? lValue = MatchToken(SyntaxType.OPEN_PARENTHESIS_TOKEN);
            Expression? expression = ParseExpression();
            Token? rValue = MatchToken(SyntaxType.CLOSE_PARENTHESIS_TOKEN);
            return new ParenthesisExpression(SyntaxTree, lValue, expression, rValue);
        }

        private Expression ParseNameEpression()
        {
            Token Identifier = MatchToken(SyntaxType.IDENTIFIER_TOKEN);
            return new NameExpression(SyntaxTree, Identifier);
        }

        private Expression ParseNameOrCallEpression()
        {
            if (Current.SyntaxType == SyntaxType.IDENTIFIER_TOKEN &&
                Peek(1).SyntaxType == SyntaxType.OPEN_PARENTHESIS_TOKEN)
            {
                return ParseCallExpression();
            }

            return ParseNameEpression();
        }

        private Expression ParseCallExpression()
        {
            // TODO :: Add threaded call support
            Token identifier = MatchToken(SyntaxType.IDENTIFIER_TOKEN);
            Token openParenthesis = MatchToken(SyntaxType.OPEN_PARENTHESIS_TOKEN);
            SeparatedSyntaxList<Expression>? arguments = ParseArguments();
            Token closenParenthesis = MatchToken(SyntaxType.CLOSE_PARENTHESIS_TOKEN);

            return new CallExpression(SyntaxTree, identifier, openParenthesis, arguments, closenParenthesis);
        }

        #region Lists
        private SeparatedSyntaxList<Expression> ParseArguments()
        {
            ImmutableArray<Node>.Builder? nodesAndSeparators = ImmutableArray.CreateBuilder<Node>();

            bool parseNextArgument = true;
            while (parseNextArgument &&
                Current.SyntaxType is not SyntaxType.END_OF_FILE_TOKEN &&
                Current.SyntaxType is not SyntaxType.CLOSE_PARENTHESIS_TOKEN)
            {
                Expression expression = ParseExpression();

                nodesAndSeparators.Add(expression);
                if (Current.SyntaxType == SyntaxType.COMMA_TOKEN)
                {
                    Token comma = MatchToken(SyntaxType.COMMA_TOKEN);
                    nodesAndSeparators.Add(comma);
                    continue;
                }

                parseNextArgument = false;
            }

            return new SeparatedSyntaxList<Expression>(nodesAndSeparators.ToImmutable());
        }

        private SeparatedSyntaxList<Parameter> ParseParameterList()
        {
            ImmutableArray<Node>.Builder? nodesAndSeparators = ImmutableArray.CreateBuilder<Node>();

            bool parseNextParameter = true;
            while (parseNextParameter &&
                Current.SyntaxType is not SyntaxType.END_OF_FILE_TOKEN &&
                Current.SyntaxType is not SyntaxType.CLOSE_PARENTHESIS_TOKEN)
            {
                Parameter parameter = ParseParameter();

                nodesAndSeparators.Add(parameter);
                if (Current.SyntaxType == SyntaxType.COMMA_TOKEN)
                {
                    Token comma = MatchToken(SyntaxType.COMMA_TOKEN);
                    nodesAndSeparators.Add(comma);
                    continue;
                }

                parseNextParameter = false;
            }

            return new SeparatedSyntaxList<Parameter>(nodesAndSeparators.ToImmutable());
        }
        #endregion
    }
}
