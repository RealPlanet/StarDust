using StarDust.Code.Symbols;
using StarDust.Code.Syntax;
using StarDust.Code.Text;
using System.Collections;
using System.Reflection.Metadata;

namespace StarDust.Code
{
    public sealed class ReportBag : IEnumerable<Report>
    {
        private readonly List<Report> ReportList = new();
        private void ReportMessage(TextLocation location, string message)
        {
            Report? report = Report.Error(location, message);
            ReportList.Add(report);
        }

        private void ReportWarning(TextLocation location, string message)
        {
            Report? report = Report.Warning(location, message);
            ReportList.Add(report);
        }

        public IEnumerator<Report> GetEnumerator()
        {
            return ReportList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddRange(IEnumerable<Report> otherReport)
        {
            ReportList.AddRange(otherReport);
        }

        public void ReportInvalidNumber(TextLocation location, string Text, TypeSymbol type)
        {
            string message = $"The number {Text} is not a valid <{type}>.";
            ReportMessage(location, message);
        }

        public void ReportBadCharacter(TextLocation location, char CurrentCharacter)
        {
            string message = $"Bad character input <{CurrentCharacter}>.";
            ReportMessage(location, message);
        }

        public void ReportUnterminatedString(TextLocation location)
        {
            const string message = "Unterminated string literal.";
            ReportMessage(location, message);
        }

        public void ReportUnterminatedMultiLineComment(TextLocation location)
        {
            const string message = "Unterminated multi-line comment.";
            ReportMessage(default, message);
        }

        public void ReportUnexpectedToken(TextLocation location, SyntaxType ReceivedKind, SyntaxType ExpectedKind)
        {
            string message = $"Unexpected token:<{ReceivedKind}>, expected <{ExpectedKind}>.";
            ReportMessage(location, message);
        }

        public void ReportUndefinedBinaryOperator(TextLocation location, string OperatorText, TypeSymbol leftType, TypeSymbol rightType)
        {
            string message = $"Binary operator <{OperatorText}> is not defined for types <{leftType}> and <{rightType}>.";
            ReportMessage(location, message);
        }
        public void ReportUndefinedVariable(TextLocation location, string name)
        {
            string message = $"Variable name does not exist <{name}>.";
            ReportMessage(location, message);
        }

        public void ReportNotAVariable(TextLocation location, string name)
        {
            string? message = $"<{name}> is not a variable.";
            ReportMessage(location, message);
        }

        public void ReportUndefinedFunction(TextLocation location, string functionName)
        {
            string message = $"Function <{functionName}> does not exist.";
            ReportMessage(location, message);
        }

        public void ReportNotAFunction(TextLocation location, string name)
        {
            string? message = $"<{name}> is not a function.";
            ReportMessage(location, message);
        }

        public void ReportUndefinedType(TextLocation location, string name)
        {
            string message = $"Type <{name}> does not exist.";
            ReportMessage(location, message);
        }

        public void ReportAllPathsMustReturn(TextLocation location)
        {
            const string message = "Not all code paths return a value";
            ReportMessage(location, message);
        }

        public void ReportUndefinedUnaryOperator(TextLocation location, string Text, TypeSymbol BoundType)
        {
            string message = $"Unary operator <{Text}> is not defined for type <{BoundType}>.";
            ReportMessage(location, message);
        }

        public void ReportSymbolAlreadyDeclared(TextLocation location, string name)
        {
            string message = $"<{name}> is already declared.";
            ReportMessage(location, message);
        }

        public void ReportParameterAlreadyDeclared(TextLocation location, string name)
        {
            string message = $"Parameter <{name}> is already declared.";
            ReportMessage(location, message);
        }

        public void ReportCannotConvertType(TextLocation location, TypeSymbol fromType, TypeSymbol toType)
        {
            string message = $"Cannot convert type of <{fromType}> to <{toType}>.";
            ReportMessage(location, message);
        }

        public void ReportCannotConvertTypeImplicity(TextLocation location, TypeSymbol fromType, TypeSymbol toType)
        {
            string message = $"Cannot convert type of <{fromType}> to <{toType}>. An explicit conversion exists (are you missing a cast?).";
            ReportMessage(location, message);
        }

        public void ReportCannotAssign(TextLocation location, string name)
        {
            string message = $"Cannot reassign value of read-only variable <{name}>.";
            ReportMessage(location, message);
        }

        public void ReportExpresionMustHaveValue(TextLocation location)
        {
            const string message = "Expression must have a value.";
            ReportMessage(location, message);
        }

        public void ReportWrongNumberOfArguments(TextLocation location, string functionName, int expectedCount, int actualCount)
        {
            string message = $"Function <{functionName}> requires {expectedCount} arguments but was given {actualCount}.";
            ReportMessage(location, message);
        }

        public void ReportInvalidBreakOrContinue(TextLocation location, string? text)
        {
            string message = $"The keyword <{text}> can only be used inside loops.";
            ReportMessage(location, message);
        }

        public void ReportInvalidReturnExpression(TextLocation location, string functionName)
        {
            string message = $"Function <{functionName}> does not have a return type. <return> keyword cannot be followed by an expression";
            ReportMessage(location, message);
        }

        public void ReportInvalidReturnWithValueInGlobalStatements(TextLocation location)
        {
            const string message = "The 'return' keyword cannot be followed by an expression in global statements.";
            ReportMessage(location, message);
        }

        public void ReportMissingReturnExpression(TextLocation location, string functionName, TypeSymbol symbol)
        {
            string message = $"Function <{functionName}> expects a return type of <{symbol}> but no expression was found after <return> keyword.";
            ReportMessage(location, message);
        }

        public void ReportInvalidExpressionStatement(TextLocation location)
        {
            //string message = $"Only assignment, call, increment, decrement, await, and new object expressions can be used as a statement.";
            const string message = "Only assignment and call expressions can be used as a statement.";
            ReportMessage(location, message);
        }

        public void ReportCannotMixMainAndGlobalStatements(TextLocation location)
        {
            const string message = "Cannot declare a main function when global statements are used.";
            ReportMessage(location, message);
        }

        public void ReportMustHaveCorrectSignature(TextLocation location)
        {
            const string message = "Main function must not take arguments and not return anything.";
            ReportMessage(location, message);
        }
        public void ReportOnlyOneFileCanHaveGlobalStatements(TextLocation location)
        {
            const string message = "Only one file can have global statements.";
            ReportMessage(location, message);
        }
        public void ReportInvalidReference(string path)
        {
            string message = $"The reference is not a valid .NET Assembly: {path}.";
            ReportMessage(default, message);
        }
        public void ReportRequiredTypeNotFound(string? langName, string metadataName)
        {
            string message = langName is not null
                ? $"The required type <{langName}> (<{metadataName}>)cannot be resolved among the given references."
                : $"The required type <{metadataName}> cannot be resolved among the given references.";
            ReportMessage(default, message);
        }

        public void ReportRequiredTypeAmbiguous(string? langName, string metadataName, TypeDefinition[] foundTypes)
        {
            //IEnumerable<string>? assemblyNames = foundTypes.Select(t => t.Module.Assembly.Name.Name);
            //string? assemblyNameList = string.Join(", ", assemblyNames);
            //
            //string message = langName is not null
            //    ? $"The required type <{langName}> (<{metadataName}>) was found in multiple references: {assemblyNameList}."
            //    : $"The required type <{metadataName}> was found in multiple references: {assemblyNameList}.";
            //ReportMessage(default, message);
        }

        public void ReportRequiredMethodNotFound(string typeName, string methodName, string[] parameterTypeNames)
        {
            string? parameterTypeNameList = string.Join(", ", parameterTypeNames);

            string message = $"The required method <{typeName}.{methodName}({parameterTypeNameList})> cannot be resolved among the given references.";
            ReportMessage(default, message);
        }

        public void ReportUnreachableCode(TextLocation location)
        {
            const string message = "Unreachable code detected.";
            ReportWarning(location, message);
        }

        public void ReportUnreachableCode(Node node)
        {
            switch (node.SyntaxType)
            {
                case SyntaxType.BLOCK_STATEMENT:
                    Statement? firstStatement = ((BlockStatement)node).Statements.FirstOrDefault();
                    // Report just for non empty blocks.
                    if (firstStatement != null)
                        ReportUnreachableCode(firstStatement);
                    return;
                case SyntaxType.VARIABLE_DECLARATION:
                    ReportUnreachableCode(((VariableDeclaration)node).Keyword.Location);
                    return;
                case SyntaxType.IF_STATEMENT:
                    ReportUnreachableCode(((IfStatement)node).IfKeyword.Location);
                    return;
                case SyntaxType.WHILE_STATEMENT:
                    ReportUnreachableCode(((WhileStatement)node).WhileKeyword.Location);
                    return;
                case SyntaxType.DO_WHILE_STATEMENT:
                    ReportUnreachableCode(((DoWhileStatement)node).DoKeyword.Location);
                    return;
                case SyntaxType.FOR_STATEMENT:
                    ReportUnreachableCode(((ForStatement)node).Keyword.Location);
                    return;
                case SyntaxType.BREAK_STATEMENT:
                    ReportUnreachableCode(((BreakStatement)node).Keyword.Location);
                    return;
                case SyntaxType.CONTINUE_STATEMENT:
                    ReportUnreachableCode(((ContinueStatement)node).Keyword.Location);
                    return;
                case SyntaxType.RETURN_STATEMENT:
                    ReportUnreachableCode(((ReturnStatement)node).ReturnKeyword.Location);
                    return;
                case SyntaxType.EXPRESSION_STATEMENT:
                    Expression? expression = ((ExpressionStatement)node).Expression;
                    ReportUnreachableCode(expression);
                    return;
                case SyntaxType.CALL_EXPRESSION_NODE:
                    ReportUnreachableCode(((CallExpression)node).Identifier.Location);
                    return;
                default:
                    throw new Exception($"Unexpected syntax {node.SyntaxType}");
            }
        }
    }
}
