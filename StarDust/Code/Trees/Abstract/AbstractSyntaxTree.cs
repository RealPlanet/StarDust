using StarDust.Code.AST.ControlFlow;
using StarDust.Code.AST.Data;
using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Expressions.Expressions;
using StarDust.Code.AST.Nodes;
using StarDust.Code.AST.Statements;
using StarDust.Code.Extensions;
using StarDust.Code.Lowering;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;
using StarDust.Code.Text;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace StarDust.Code.AST
{
    internal sealed class AbstractSyntaxTree
    {
        #region Public member vars
        public ReportBag Report { get; } = new();
        public bool IsScript { get; }
        public FunctionSymbol? Function { get; }
        #endregion

        #region Private member vars
        private readonly Stack<(AbstractLabel BreakLabel, AbstractLabel ContinueLabel)> LoopStack = new();
        private int LabelCounter = 0;
        private AbstractScope Scope;
        #endregion

        private AbstractSyntaxTree(bool isScript, AbstractScope? parent, FunctionSymbol? function = null)
        {
            Scope = new AbstractScope(parent);
            IsScript = isScript;
            Function = function;

            if (Function is not null)
            {
                foreach (ParameterSymbol? p in Function.Parameters)
                    Scope.TryDeclareVariable(p);
            }
        }

        #region Static methods
        public static AbstractGlobalScope BindGlobalScope(bool isScript, AbstractGlobalScope? previous, ImmutableArray<ConcreteSyntaxTree> syntaxTrees)
        {
            AbstractScope? parentScope = CreateParentScope(previous);
            AbstractSyntaxTree binder = new(isScript, parentScope);

            binder.Report.AddRange(syntaxTrees.SelectMany(st => st.Report));
            if (binder.Report.Any())
            {
                return new(previous, binder.Report.ToImmutableArray(), null, null,
                    ImmutableArray<FunctionSymbol>.Empty, ImmutableArray<VariableSymbol>.Empty, ImmutableArray<AbstractStatement>.Empty);
            }

            IEnumerable<FunctionDeclaration>? functionDeclarations = syntaxTrees.SelectMany(st => st.Root.Members).OfType<FunctionDeclaration>();
            foreach (FunctionDeclaration? function in functionDeclarations)
                binder.BindFunctionDeclaration(function);

            IEnumerable<GlobalStatement>? globalStatements = syntaxTrees.SelectMany(st => st.Root.Members).OfType<GlobalStatement>();

            ImmutableArray<AbstractStatement>.Builder? statementBuilder = ImmutableArray.CreateBuilder<AbstractStatement>();

            foreach (GlobalStatement? globalStatement in globalStatements)
            {
                AbstractStatement? s = binder.BindGlobalStatement(globalStatement.Statement);
                statementBuilder.Add(s);
            }

            // Global statemens can only occour in at most one file
            GlobalStatement?[]? firstGlobalStatementPerSyntaxTree = syntaxTrees.Select(st => st.Root.Members.OfType<GlobalStatement>().FirstOrDefault())
                                                                    .Where(g => g is not null)
                                                                    .ToArray();
            if (firstGlobalStatementPerSyntaxTree.Length > 1)
            {
                foreach (GlobalStatement? gs in firstGlobalStatementPerSyntaxTree)
                    binder.Report.ReportOnlyOneFileCanHaveGlobalStatements(gs!.Location);
            }

            // if a main function esists, global statements cannot exist
            ImmutableArray<FunctionSymbol> functions = binder.Scope.GetDeclaredFunctions();

            FunctionSymbol? mainFunction = null;
            FunctionSymbol? scriptFunction = null;

            if (isScript)
            {
                if (globalStatements.Any())
                    scriptFunction = new("$eval", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.Any, null);
            }
            else
            {
                mainFunction = functions.FirstOrDefault(f => f.Name == "main");
                if(mainFunction is null)
                {
                    throw new Exception();
                }

                if (mainFunction is not null)
                {
                    if (mainFunction.ReturnType != TypeSymbol.Void || mainFunction.Parameters.Any())
                        binder.Report.ReportMustHaveCorrectSignature(mainFunction.Declaration!.Identifier.Location);
                }

                if (globalStatements.Any())
                {
                    if (mainFunction is not null)
                    {
                        binder.Report.ReportCannotMixMainAndGlobalStatements(mainFunction.Declaration!.Identifier.Location);
                        foreach (GlobalStatement? gs in firstGlobalStatementPerSyntaxTree)
                            binder.Report.ReportCannotMixMainAndGlobalStatements(gs!.Location);
                    }
                    else
                    {
                        mainFunction = new("main", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.Void, null);
                    }
                }
            }

            ImmutableArray<Report> report = binder.Report.ToImmutableArray();
            ImmutableArray<VariableSymbol> variables = binder.Scope.GetDeclaredVariables();

            if (previous != null)
                report.InsertRange(0, previous.Report);

            return new AbstractGlobalScope(previous, report, mainFunction, scriptFunction, functions, variables, statementBuilder.ToImmutable());
        }
        public static AbstractProgram BindProgram(bool isScript, AbstractProgram? previous, AbstractGlobalScope globalScope)
        {
            AbstractScope parentScope = CreateParentScope(globalScope);

            if (globalScope.Report.Any())
                return new(previous, globalScope.Report, null, null, ImmutableDictionary<FunctionSymbol, AbstractBlockStatement>.Empty);

            ImmutableDictionary<FunctionSymbol, AbstractBlockStatement>.Builder? functionBodies = ImmutableDictionary.CreateBuilder<FunctionSymbol, AbstractBlockStatement>();
            ReportBag report = new();

            foreach (FunctionSymbol? function in globalScope.Functions)
            {
                AbstractSyntaxTree binder = new(isScript, parentScope, function);
                AbstractStatement? body = binder.BindStatement(function.Declaration!.Body);
                AbstractBlockStatement? loweredBody = Lowerer.Lower(function, body);
                if (function.ReturnType != TypeSymbol.Void && !ControlFlowGraph.AllPathsReturn(loweredBody))
                    binder.Report.ReportAllPathsMustReturn(function.Declaration.Identifier.Location);

                functionBodies.Add(function, loweredBody);
                report.AddRange(binder.Report);
            }

            Node? compilationUnit = globalScope.Statements.Any()
                                    ? globalScope.Statements.First().Syntax.AncestorsAndSelf().LastOrDefault()
                                    : null;

            if (globalScope.MainFunction != null && globalScope.Statements.Any())
            {
                AbstractBlockStatement body = Lowerer.Lower(globalScope.MainFunction, new AbstractBlockStatement(compilationUnit!, globalScope.Statements));
                functionBodies.Add(globalScope.MainFunction, body);
            }
            else if (globalScope.ScriptFunction is not null)
            {
                ImmutableArray<AbstractStatement> statements = globalScope.Statements;
                if (statements.Length == 1 &&
                    statements[0] is AbstractExpressionStatement es &&
                    es.Expression.Type != TypeSymbol.Void)
                {
                    statements = statements.SetItem(0, new AbstractReturnStatement(es.Expression.Syntax, es.Expression));
                }
                else if (statements.Any() && statements.Last().NodeType != AbstractNodeType.RETURN_STATEMENT)
                {
                    AbstractLiteralExpression nullValue = new(compilationUnit!, "");
                    statements = statements.Add(new AbstractReturnStatement(compilationUnit!, nullValue));
                }

                AbstractBlockStatement? body = Lowerer.Lower(globalScope.ScriptFunction, new AbstractBlockStatement(compilationUnit!, statements));
                functionBodies.Add(globalScope.ScriptFunction, body);
            }

            AbstractProgram boundProgram = new(previous, report.ToImmutableArray(), globalScope.MainFunction, globalScope.ScriptFunction, functionBodies.ToImmutable());
            return boundProgram;
        }
        private static AbstractStatement BindErrorStatement(Node syntax)
        {
            return new AbstractExpressionStatement(syntax, new AbstractErrorExpression(syntax));
        }

        private static AbstractScope CreateParentScope(AbstractGlobalScope? previous)
        {
            // chain (in REPL) : submission 3 -> submission 2 -> submission 1 -> global

            Stack<AbstractGlobalScope> stack = new();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }

            AbstractScope parent = CreateRootScope();
            while (stack.Count > 0)
            {
                previous = stack.Pop();
                AbstractScope scope = new(parent);

                foreach (FunctionSymbol? f in previous.Functions)
                    scope.TryDeclareFunction(f);

                foreach (VariableSymbol? v in previous.Variables)
                    scope.TryDeclareVariable(v);

                parent = scope;
            }

            return parent;
        }
        private static AbstractScope CreateRootScope()
        {
            AbstractScope result = new(null);
            //foreach (FunctionSymbol? f in BuiltinFunction.GetAll())
            //    result.TryDeclareFunction(f);
            return result;
        }
        private static TypeSymbol? LookupType(string name)
        {
            return name switch
            {
                "void"      => TypeSymbol.Void,
                "int"       => TypeSymbol.Int,
                "bool"      => TypeSymbol.Bool,
                "string"    => TypeSymbol.String,
                "any"       => TypeSymbol.Any,
                _ => null,
            };
        }
        #endregion

        #region Private methods
        private AbstractStatement BindGlobalStatement(Statement syntax)
        {
            return BindStatement(syntax, true);
        }

        private AbstractStatement BindStatement(Statement syntax, bool isGlobal = false)
        {
            AbstractStatement? result = BindStatementInternal(syntax);

            if (!IsScript || !isGlobal)
            {
                if (result is AbstractExpressionStatement es)
                {
                    bool isAllowedExpression = es.Expression.NodeType == AbstractNodeType.ERROR_EXPRESSION ||
                                                es.Expression.NodeType == AbstractNodeType.ASSIGNMENT_EXPRESSION ||
                                                es.Expression.NodeType == AbstractNodeType.CALL_EXPRESSION ||
                                                es.Expression.NodeType == AbstractNodeType.COMPOUND_ASSIGNMENT_EXPRESSION; ;
                    if (!isAllowedExpression)
                        Report.ReportInvalidExpressionStatement(syntax.Location);
                }
            }

            return result;
        }
        private AbstractStatement BindStatementInternal(Statement syntax)
        {
            return syntax.SyntaxType switch
            {
                SyntaxType.BLOCK_STATEMENT => BindBlockStatement((BlockStatement)syntax),
                SyntaxType.EXPRESSION_STATEMENT => BindExpressionStatement((ExpressionStatement)syntax),
                SyntaxType.IF_STATEMENT => BindIfStatement((IfStatement)syntax),
                SyntaxType.WHILE_STATEMENT => BindWhileStatement((WhileStatement)syntax),
                SyntaxType.DO_WHILE_STATEMENT => BindDoWhileStatement((DoWhileStatement)syntax),
                SyntaxType.FOR_STATEMENT => BindForStatement((ForStatement)syntax),
                SyntaxType.BREAK_STATEMENT => BindBreakStatement((BreakStatement)syntax),
                SyntaxType.CONTINUE_STATEMENT => BindContinueStatement((ContinueStatement)syntax),
                SyntaxType.RETURN_STATEMENT => BindReturnStatement((ReturnStatement)syntax),

                SyntaxType.VARIABLE_DECLARATION => BindVariableDeclaration((VariableDeclaration)syntax),
                _ => throw new Exception($"Unexpected syntax <{syntax.SyntaxType}>"),
            };
        }

        private AbstractExpression BindExpression(Expression syntax, TypeSymbol targetType)
        {
            return BindConversion(syntax, targetType);
        }

        private AbstractExpression BindExpression(Expression syntax, bool canBeVoid = false)
        {
            AbstractExpression result = BindExpressionInternal(syntax);
            if (!canBeVoid && result.Type == TypeSymbol.Void)
            {
                Report.ReportExpresionMustHaveValue(syntax.Location);
                return new AbstractErrorExpression(syntax);
            }

            return result;
        }
        private AbstractExpression BindConversion(Expression syntax, TypeSymbol type, bool allowExplicit = false)
        {
            AbstractExpression expression = BindExpression(syntax);
            return BindConversion(syntax.Location, expression, type, allowExplicit);
        }
        private AbstractExpression BindConversion(TextLocation reportLocation, AbstractExpression expression, TypeSymbol type, bool allowExplicit = false)
        {
            Conversion conversion = Conversion.Classify(expression.Type, type);

            if (!conversion.Exists)
            {
                if (expression.Type != TypeSymbol.Error && type != TypeSymbol.Error)
                    Report.ReportCannotConvertType(reportLocation, expression.Type, type);

                return new AbstractErrorExpression(expression.Syntax);
            }

            if (!allowExplicit && conversion.IsExplicit)
            {
                Report.ReportCannotConvertTypeImplicity(reportLocation, expression.Type, type);
                return new AbstractErrorExpression(expression.Syntax);
            }

            if (conversion.IsIdentity)
                return expression;

            return new AbstractConversionExpression(expression.Syntax, type, expression);
        }
        private AbstractStatement BindExpressionStatement(ExpressionStatement syntax)
        {
            AbstractExpression expression = BindExpression(syntax.Expression, true);
            return new AbstractExpressionStatement(syntax, expression);
        }
        private AbstractExpression BindExpressionInternal(Expression syntax)
        {
            return syntax.SyntaxType switch
            {
                SyntaxType.PARENTHESIZED_EXPRESSION_NODE => BindParanthesizedExpression((ParenthesisExpression)syntax),
                SyntaxType.LITERAL_EXPRESSION_NODE => BindLiteralExpression((LiteralExpression)syntax),
                SyntaxType.UNARY_EXPRESSION_NODE => BindUnaryExpression((UnaryExpression)syntax),
                SyntaxType.BINARY_EXPRESSION_NODE => BindBinaryExpression((BinaryExpression)syntax),
                SyntaxType.NAME_EXPRESSION_NODE => BindNameExpression((NameExpression)syntax),
                SyntaxType.ASSIGNMENT_EXPRESSION_NODE => BindAssignmentExpression((AssignmentExpression)syntax),
                SyntaxType.CALL_EXPRESSION_NODE => BindCallExpression((CallExpression)syntax),
                _ => throw new Exception($"Unexpected syntax <{syntax.SyntaxType}>"),
            };
        }

        private void BindFunctionDeclaration(FunctionDeclaration syntax)
        {
            ImmutableArray<ParameterSymbol>.Builder parameters = ImmutableArray.CreateBuilder<ParameterSymbol>();
            HashSet<string> seenNames = new();
            foreach (Parameter? p in syntax.Parameters)
            {
                string name = p.Identifier.Text;
                TypeSymbol parameterType = BindTypeClause(p.Type);
                if (!seenNames.Add(name))
                {
                    Report.ReportParameterAlreadyDeclared(p.Location, name);
                    continue;
                }

                ParameterSymbol parameter = new(name, parameterType, parameters.Count);
                parameters.Add(parameter);
            }

            TypeSymbol type = BindTypeClause(syntax.Type) ?? TypeSymbol.Void;

            FunctionSymbol function = new(syntax.Identifier.Text, parameters.ToImmutable(), type, syntax);
            if (syntax.Identifier.Text != null && !Scope.TryDeclareFunction(function))
            {
                Report.ReportSymbolAlreadyDeclared(syntax.Identifier.Location, function.Name);
            }
        }

        #endregion

        #region Binders
        private AbstractStatement BindReturnStatement(ReturnStatement syntax)
        {
            // Does the function have a return type?
            // Does the return type match?
            // Are we inside/outside a function?

            AbstractExpression? expression = syntax.Expression is null ? null : BindExpression(syntax.Expression);
            if (Function is null)
            {
                // Is script is true within the repl, in all other cases it should either lower global statements into a main function or
                // report an error due to multiple files with global statements
                if (IsScript)
                {
                    if (expression is null)
                    {
                        expression = new AbstractLiteralExpression(syntax, "");
                    }
                }
                else if (expression is not null)
                {
                    //Main does not support return value
                    // Global statements are lowered into a main function if the compiler encounters a file with global statements
                    Report.ReportInvalidReturnWithValueInGlobalStatements(syntax.Expression!.Location);
                }
            }
            else
            {
                // If a function does not return anything then we shouldn't be here to begin with.
                if (Function.ReturnType == TypeSymbol.Void)
                {
                    if (expression is not null)
                        Report.ReportInvalidReturnExpression(syntax.Expression!.Location, Function.Name);
                }
                else
                {
                    // A return statement always needs an expression if return type is not void
                    if (expression is null)
                        Report.ReportMissingReturnExpression(syntax.ReturnKeyword.Location, Function.Name, Function.ReturnType);
                    else
                        expression = BindConversion(syntax.Expression!.Location, expression, Function.ReturnType);
                }
            }

            return new AbstractReturnStatement(syntax, expression);
        }
        private AbstractExpression BindCallExpression(CallExpression syntax)
        {
            // Type casters
            if (syntax.Arguments.Count == 1 && LookupType(syntax.Identifier.Text) is TypeSymbol type)
                return BindConversion(syntax.Arguments[0], type, allowExplicit: true);

            ImmutableArray<AbstractExpression>.Builder? boundArguments = ImmutableArray.CreateBuilder<AbstractExpression>();
            foreach (Expression? argument in syntax.Arguments)
            {
                AbstractExpression? boundArgument = BindExpression(argument);
                boundArguments.Add(boundArgument);
            }

            //if (!Scope.TryLookupFunction(syntax.Identifier.Text, out FunctionSymbol? function))
            Symbol? symbol = Scope.TryLookupSymbol(syntax.Identifier.Text);
            if (symbol == null)
            {
                Report.ReportUndefinedFunction(syntax.Identifier.Location, syntax.Identifier.Text);
                return new AbstractErrorExpression(syntax);
            }

            if (symbol is not FunctionSymbol function)
            {
                Report.ReportNotAFunction(syntax.Identifier.Location, syntax.Identifier.Text);
                return new AbstractErrorExpression(syntax);
            }

            if (syntax.Arguments.Count != function.Parameters.Length)
            {
                TextSpan span;
                if (syntax.Arguments.Count > function.Parameters.Length)
                {
                    Node firstExceedingNode;
                    if (function.Parameters.Length > 0)
                        firstExceedingNode = syntax.Arguments.GetSeparator(function.Parameters.Length - 1);
                    else
                        firstExceedingNode = syntax.Arguments[0];

                    Expression? lastExceedingArgument = syntax.Arguments[^1];
                    span = TextSpan.FromBounds(firstExceedingNode.Span.Start, lastExceedingArgument.Span.End);
                }
                else
                {
                    span = syntax.CloseParenthesis.Span;
                }
                TextLocation location = new(syntax.SyntaxTree.Text, span);
                Report.ReportWrongNumberOfArguments(location, function.Name, function.Parameters.Length, syntax.Arguments.Count);
                return new AbstractErrorExpression(syntax);
            }

            for (int i = 0; i < syntax.Arguments.Count; i++)
            {
                TextLocation argumentLocation = syntax.Arguments[i].Location;
                AbstractExpression argument = boundArguments[i];
                ParameterSymbol? parameter = function.Parameters[i];
                boundArguments[i] = BindConversion(argumentLocation, argument, parameter.Type);
            }

            return new AbstractCallExpression(syntax, function, boundArguments.ToImmutable());
        }
        private AbstractStatement BindForStatement(ForStatement syntax)
        {
            AbstractExpression lowerBound = BindExpression(syntax.LowerBound, TypeSymbol.Int);
            AbstractExpression upperBound = BindExpression(syntax.UpperBound, TypeSymbol.Int);
            Scope = new AbstractScope(Scope);

            Token identifier = syntax.Identifier;
            VariableSymbol variable = BindVariableDeclaration(identifier, true, TypeSymbol.Int);

            AbstractStatement body = BindLoopBody(syntax.Body, out AbstractLabel breakLabel, out AbstractLabel continueLabel);

            // This will always be non-null, see above.
            Scope = Scope.Parent!;

            return new AbstractForStatement(syntax, variable, lowerBound, upperBound, body, breakLabel, continueLabel);
        }
        private AbstractStatement BindWhileStatement(WhileStatement syntax)
        {
            AbstractExpression condition = BindExpression(syntax.Condition, TypeSymbol.Bool);

            if (condition.ConstantValue != null)
            {
                if (!(bool)condition.ConstantValue.Value)
                {
                    Report.ReportUnreachableCode(syntax.Body);
                }
            }

            AbstractStatement body = BindLoopBody(syntax.Body, out AbstractLabel breakLabel, out AbstractLabel continueLabel);
            return new AbstractWhileStatement(syntax, condition, body, breakLabel, continueLabel);
        }
        private AbstractStatement BindDoWhileStatement(DoWhileStatement syntax)
        {
            AbstractStatement body = BindLoopBody(syntax.Body, out AbstractLabel breakLabel, out AbstractLabel continueLabel);
            AbstractExpression condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            return new AbstractDoWhileStatement(syntax, body, condition, breakLabel, continueLabel);
        }
        private AbstractStatement BindBreakStatement(BreakStatement syntax)
        {
            if (LoopStack.Count == 0)
            {
                Report.ReportInvalidBreakOrContinue(syntax.Keyword.Location, syntax.Keyword.Text);
                return BindErrorStatement(syntax);
            }

            AbstractLabel breakLabel = LoopStack.Peek().BreakLabel;
            return new AbstractGotoStatement(syntax, breakLabel);
        }
        private AbstractStatement BindContinueStatement(ContinueStatement syntax)
        {
            if (LoopStack.Count == 0)
            {
                Report.ReportInvalidBreakOrContinue(syntax.Keyword.Location, syntax.Keyword.Text);
                return BindErrorStatement(syntax);
            }

            AbstractLabel continueLabel = LoopStack.Peek().ContinueLabel;
            return new AbstractGotoStatement(syntax, continueLabel);
        }
        private AbstractStatement BindLoopBody(Statement body, out AbstractLabel breakLabel, out AbstractLabel continueLabel)
        {
            LabelCounter++;
            breakLabel = new AbstractLabel($"Break{LabelCounter}");
            continueLabel = new AbstractLabel($"Continue{LabelCounter}");

            LoopStack.Push((breakLabel, continueLabel));
            AbstractStatement result = BindStatement(body);
            LoopStack.Pop();
            return result;
        }
        private AbstractStatement BindIfStatement(IfStatement syntax)
        {
            AbstractExpression condition = BindExpression(syntax.Condition, TypeSymbol.Bool);

            if (condition.ConstantValue != null)
            {
                if ((bool)condition.ConstantValue.Value == false)
                    Report.ReportUnreachableCode(syntax.ThenStatement);
                else if (syntax.ElseClause != null)
                    Report.ReportUnreachableCode(syntax.ElseClause.ElseStatement);
            }

            AbstractStatement thenStatement = BindStatement(syntax.ThenStatement);
            AbstractStatement? elseStatement = syntax.ElseClause == null ? null : BindStatement(syntax.ElseClause.ElseStatement);
            return new AbstractIfStatement(syntax, condition, thenStatement, elseStatement);
        }
        private AbstractStatement BindVariableDeclaration(VariableDeclaration syntax)
        {
            bool isReadOnly = syntax.Keyword.SyntaxType == SyntaxType.CONST_KEYWORD;
            TypeSymbol? type = BindTypeClause(syntax.Type);
            AbstractExpression initializer = BindExpression(syntax.Initializer);
            TypeSymbol variableType = type ?? initializer.Type;
            VariableSymbol variable = BindVariableDeclaration(syntax.Identifier, isReadOnly, variableType, initializer.ConstantValue);
            AbstractExpression? convertedInitializer = BindConversion(syntax.Initializer.Location, initializer, variableType);

            return new AbstractVariableDeclaration(syntax, variable, convertedInitializer);
        }
        private VariableSymbol BindVariableDeclaration(Token identifier, bool isReadOnly, TypeSymbol type, AbstractConstant? constant = null)
        {
            string? name = identifier.Text ?? "?";
            bool declare = !identifier.IsMissing;
            VariableSymbol variable = Function == null
                                        ? new GlobalVariableSymbol(name, isReadOnly, type, constant)
                                        : new LocalVariableSymbol(name, isReadOnly, type, constant);

            // Should never happen as shadowing is allowed and we created a new scope
            if (declare && !Scope.TryDeclareVariable(variable))
                Report.ReportSymbolAlreadyDeclared(identifier.Location, name);

            return variable;
        }
        private VariableSymbol? BindVariableReference(Token identifierToken)
        {
            string name = identifierToken.Text;
            switch (Scope.TryLookupSymbol(name))
            {
                case VariableSymbol variable:
                    return variable;

                case null:
                    Report.ReportUndefinedVariable(identifierToken.Location, name);
                    return null;

                default:
                    Report.ReportNotAVariable(identifierToken.Location, name);
                    return null;
            }
        }

        [return: NotNullIfNotNull("typeClause")]
        private TypeSymbol? BindTypeClause(TypeClause? typeClause)
        {
            if (typeClause is null)
                return null;

            TypeSymbol? type = LookupType(typeClause.Identifier.Text);
            if (type is null)
                Report.ReportUndefinedType(typeClause.Identifier.Location, typeClause.Identifier.Text);

            return type!;
        }
        private AbstractStatement BindBlockStatement(BlockStatement syntax)
        {
            ImmutableArray<AbstractStatement>.Builder? statements = ImmutableArray.CreateBuilder<AbstractStatement>();
            Scope = new AbstractScope(Scope);
            foreach (Statement? s in syntax.Statements)
            {
                AbstractStatement? statement = BindStatement(s);
                statements.Add(statement);
            }

            // This will always be non-null, see above.
            Scope = Scope.Parent!;
            return new AbstractBlockStatement(syntax, statements.ToImmutable());
        }
        private AbstractExpression BindParanthesizedExpression(ParenthesisExpression syntax)
        {
            return BindExpression(syntax.Expression);
        }

        private static AbstractExpression BindLiteralExpression(LiteralExpression syntax)
        {
            object value = syntax.Value ?? 0;
            return new AbstractLiteralExpression(syntax, value);
        }
        private AbstractExpression BindUnaryExpression(UnaryExpression syntax)
        {
            AbstractExpression boundOperand = BindExpression(syntax.Operand);
            if (boundOperand.Type == TypeSymbol.Error)
                return new AbstractErrorExpression(syntax);

            AbstractUnaryOperator? boundOperator = AbstractUnaryOperator.Bind(syntax.OperatorToken.SyntaxType, boundOperand.Type);

            if (boundOperator is null)
            {
                Report.ReportUndefinedUnaryOperator(syntax.OperatorToken.Location, syntax.OperatorToken.Text, boundOperand.Type);
                return new AbstractErrorExpression(syntax);
            }

            return new AbstractUnaryExpression(syntax, boundOperator, boundOperand);
        }
        private AbstractExpression BindBinaryExpression(BinaryExpression syntax)
        {
            AbstractExpression boundLeft = BindExpression(syntax.Left);
            AbstractExpression boundRight = BindExpression(syntax.Right);

            if (boundLeft.Type == TypeSymbol.Error || boundRight.Type == TypeSymbol.Error)
                return new AbstractErrorExpression(syntax);

            AbstractBinaryOperator? BoundOperatorType = AbstractBinaryOperator.Bind(syntax.OperatorToken.SyntaxType, boundLeft.Type, boundRight.Type);
            if (BoundOperatorType is null)
            {
                Report.ReportUndefinedBinaryOperator(syntax.OperatorToken.Location, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
                return new AbstractErrorExpression(syntax);
            }

            return new AbstractBinaryExpression(syntax, boundLeft, BoundOperatorType, boundRight);
        }
        private AbstractExpression BindNameExpression(NameExpression syntax)
        {
            if (syntax.IdentifierToken.IsMissing)
            {
                // This means the token was inserted by the parser and already reported the error
                return new AbstractErrorExpression(syntax);
            }

            VariableSymbol? variable = BindVariableReference(syntax.IdentifierToken);
            if (variable == null)
                return new AbstractErrorExpression(syntax);

            return new AbstractVariableExpression(syntax, variable);
        }
        private AbstractExpression BindAssignmentExpression(AssignmentExpression syntax)
        {
            string? name = syntax.IdentifierToken.Text;
            AbstractExpression boundExpression = BindExpression(syntax.Expression);

            VariableSymbol? variable = BindVariableReference(syntax.IdentifierToken);
            if (variable == null)
            {
                return boundExpression;
            }

            if (variable.IsReadOnly)
                Report.ReportCannotAssign(syntax.AssignmentToken.Location, name);
            AbstractExpression convertedExpression;
            if (syntax.AssignmentToken.SyntaxType != SyntaxType.EQUALS_TOKEN)
            {
                SyntaxType equivalentOperatorTokenKind = syntax.AssignmentToken.SyntaxType.GetBinaryOperatorOfAssignmentOperator();
                AbstractBinaryOperator? boundOperator = AbstractBinaryOperator.Bind(equivalentOperatorTokenKind, variable.Type, boundExpression.Type);

                if (boundOperator == null)
                {
                    Report.ReportUndefinedBinaryOperator(syntax.AssignmentToken.Location, syntax.AssignmentToken.Text, variable.Type, boundExpression.Type);
                    return new AbstractErrorExpression(syntax);
                }

                convertedExpression = BindConversion(syntax.Expression.Location, boundExpression, variable.Type);
                return new AbstractCompoundAssignmentExpression(syntax, variable, boundOperator, convertedExpression);
            }

            convertedExpression = BindConversion(syntax.Expression.Location, boundExpression, variable.Type);
            return new AbstractAssignmentExpression(syntax, variable, convertedExpression);
        }
        #endregion
    }
}
