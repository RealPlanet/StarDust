using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Expressions.Expressions;
using StarDust.Code.AST.Nodes;
using StarDust.Code.AST.Statements;
using StarDust.Code.Extensions;
using StarDust.Code.IO;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;
using StarDust.Code.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace StarDust.Code.AST.Processing
{
    internal static class AbstractNodePrinter
    {
        private static void WriteNestedStatement(this IndentedTextWriter writer, AbstractStatement node)
        {
            bool needsIndent = node is not AbstractBlockStatement;
            if (needsIndent)
            {
                writer.Indent++;
                node.WriteTo(writer);
                writer.Indent--;
                return;
            }

            node.WriteTo(writer);
        }

        private static void WriteNestedExpression(this IndentedTextWriter writer, int parentPrecedence, AbstractExpression node)
        {
            if (node is AbstractUnaryExpression unary)
                writer.WriteNestedExpression(parentPrecedence, unary.Operator.SyntaxType.GetUnaryOperatorPrecedence(), unary);
            else if (node is AbstractBinaryExpression binary)
                writer.WriteNestedExpression(parentPrecedence, binary.Operator.SyntaxType.GetBinaryOperatorPrecedence(), binary);
            else
                node.WriteTo(writer);
        }

        private static void WriteNestedExpression(this IndentedTextWriter writer, int parentPrecedence, int currentPrecedence, AbstractExpression node)
        {
            bool needsParenthesis = parentPrecedence >= currentPrecedence;
            if (needsParenthesis)
            {
                writer.WritePunctuation(SyntaxType.OPEN_PARENTHESIS_TOKEN);
                node.WriteTo(writer);
                writer.WritePunctuation(SyntaxType.CLOSE_PARENTHESIS_TOKEN);
                return;
            }

            node.WriteTo(writer);
        }

        public static void WriteTo(this AbstractNode node, System.IO.TextWriter writer)
        {
            if (writer is IndentedTextWriter iw)
                WriteTo(node, iw);
            else
                WriteTo(node, new IndentedTextWriter(writer));
        }

        public static void WriteTo(this AbstractNode node, IndentedTextWriter writer)
        {
            switch (node.NodeType)
            {
                case AbstractNodeType.BLOCK_STATEMENT:
                    WriteBlockStatement((AbstractBlockStatement)node, writer);
                    break;
                case AbstractNodeType.NOP_STATEMENT:
                    WriteNopStatement((AbstractNopStatement)node, writer);
                    break;
                case AbstractNodeType.SEQUENCE_POINT_STATEMENT:
                    WriteSequencePointStatement((BoundSequencePointStatement)node, writer);
                    break;
                case AbstractNodeType.EXPRESSION_STATEMENT:
                    WriteExpressionStatement((AbstractExpressionStatement)node, writer);
                    break;
                case AbstractNodeType.VARIABLE_DECLARATION_STATEMENT:
                    WriteVariableDeclarationStatement((AbstractVariableDeclaration)node, writer);
                    break;
                case AbstractNodeType.IF_STATEMENT:
                    WriteIfStatement((AbstractIfStatement)node, writer);
                    break;
                case AbstractNodeType.WHILE_STATEMENT:
                    WriteWhileStatement((AbstractWhileStatement)node, writer);
                    break;
                case AbstractNodeType.DO_WHILE_STATEMENT:
                    WriteDoWhileStatement((AbstractDoWhileStatement)node, writer);
                    break;
                case AbstractNodeType.FOR_STATEMENT:
                    WriteForStatement((AbstractForStatement)node, writer);
                    break;
                case AbstractNodeType.CONDITIONAL_GOTO_STATEMENT:
                    WriteConditionalGotoStatement((AbstractConditionalGotoStatement)node, writer);
                    break;
                case AbstractNodeType.GOTO_STATEMENT:
                    WriteGotoStatement((AbstractGotoStatement)node, writer);
                    break;
                case AbstractNodeType.LABEL_STATEMENT:
                    WriteLabelStatement((AbstractLabelStatement)node, writer);
                    break;
                case AbstractNodeType.RETURN_STATEMENT:
                    WriteReturnStatement((AbstractReturnStatement)node, writer);
                    break;
                case AbstractNodeType.ERROR_EXPRESSION:
                    WriteErrorExpression((AbstractErrorExpression)node, writer);
                    break;
                case AbstractNodeType.UNARY_EXPRESSION:
                    WriteUnaryExpression((AbstractUnaryExpression)node, writer);
                    break;
                case AbstractNodeType.LITERAL_EXPRESSION:
                    WriteLiteralExpression((AbstractLiteralExpression)node, writer);
                    break;
                case AbstractNodeType.BINARY_EXPRESSION:
                    WriteBinaryExpression((AbstractBinaryExpression)node, writer);
                    break;
                case AbstractNodeType.VARIABLE_EXPRESSION:
                    WriteVariableExpression((AbstractVariableExpression)node, writer);
                    break;
                case AbstractNodeType.ASSIGNMENT_EXPRESSION:
                    WriteAssignmentExpression((AbstractAssignmentExpression)node, writer);
                    break;
                case AbstractNodeType.COMPOUND_ASSIGNMENT_EXPRESSION:
                    WriteCompoundAssignmentExpression((AbstractCompoundAssignmentExpression)node, writer);
                    break;
                case AbstractNodeType.CALL_EXPRESSION:
                    WriteCallExpression((AbstractCallExpression)node, writer);
                    break;
                case AbstractNodeType.CONVERSION_EXPRESSION:
                    WriteConversionExpression((AbstractConversionExpression)node, writer);
                    break;
                default:
                    throw new Exception($"Unexpected node {node.NodeType}");
            }
        }

        private static void WriteSequencePointStatement(BoundSequencePointStatement node, IndentedTextWriter writer)
        {
            SourceText sourceText = node.Location.Text;
            TextSpan span = node.Location.Span;

            int startLine = sourceText.GetLineIndex(span.Start);
            int endLine = sourceText.GetLineIndex(span.End - 1);

            for (int i = startLine; i <= endLine; i++)
            {
                TextLine line = sourceText.Lines[i];
                int start = Math.Max(line.Start, span.Start);
                int end = Math.Min(line.End, span.End);
                TextSpan lineSpan = TextSpan.FromBounds(start, end);
                string text = sourceText.ToString(lineSpan);
                writer.WriteComment(text);
            }

            writer.WriteLine();
            node.Statement.WriteTo(writer);
        }

        private static void WriteNopStatement(AbstractNopStatement _, IndentedTextWriter writer)
        {
            writer.WriteKeyword("nop");
            writer.WriteLine();
        }

        private static void WriteReturnStatement(AbstractReturnStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(SyntaxType.RETURN_KEYWORD);
            writer.WriteSpace();
            if (node.Expression is not null)
                node.Expression.WriteTo(writer);
            else
                writer.WriteIdentifier("<void>");
            writer.WriteLine();
        }

        private static void WriteBlockStatement(AbstractBlockStatement node, IndentedTextWriter writer)
        {
            writer.WritePunctuation(SyntaxType.OPEN_BRACE_TOKEN);
            writer.WriteLine();
            writer.Indent++;
            foreach (AbstractStatement? s in node.Statements)
                s.WriteTo(writer);
            writer.Indent--;
            writer.WritePunctuation(SyntaxType.CLOSE_BRACE_TOKEN);
            writer.WriteLine();
        }
        private static void WriteExpressionStatement(AbstractExpressionStatement node, IndentedTextWriter writer)
        {
            node.Expression.WriteTo(writer);
            writer.WriteLine();
        }

        private static void WriteVariableDeclarationStatement(AbstractVariableDeclaration node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(node.Variable.IsReadOnly ? SyntaxType.CONST_KEYWORD : SyntaxType.VAR_KEYWORD);
            writer.WriteSpace();

            node.Variable.Type.WriteTo(writer);
            writer.WriteSpace();

            writer.WriteIdentifier(node.Variable.Name);
            writer.WriteSpace();
            writer.WritePunctuation(SyntaxType.EQUALS_TOKEN);
            writer.WriteSpace();
            node.Initializer.WriteTo(writer);
            writer.WriteLine();
        }

        private static void WriteIfStatement(AbstractIfStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(SyntaxType.IF_KEYWORD);
            writer.WriteSpace();
            node.Condition.WriteTo(writer);
            writer.WriteLine();
            writer.WriteNestedStatement(node.ThenStatement);
            if (node.ElseStatement != null)
            {
                writer.WriteKeyword(SyntaxType.ELSE_CLAUSE);
                writer.WriteLine();
                writer.WriteNestedStatement(node.ElseStatement);
            }
        }

        private static void WriteWhileStatement(AbstractWhileStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(SyntaxType.WHILE_KEYWORD);
            writer.WriteSpace();
            node.Condition.WriteTo(writer);
            writer.WriteLine();
            writer.WriteNestedStatement(node.Body);
        }
        private static void WriteDoWhileStatement(AbstractDoWhileStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(SyntaxType.DO_KEYWORD);
            writer.WriteLine();
            writer.WriteNestedStatement(node.Body);
            writer.WriteLine();
            writer.WriteKeyword(SyntaxType.WHILE_KEYWORD);
            writer.WriteSpace();
            node.Condition.WriteTo(writer);
        }

        private static void WriteForStatement(AbstractForStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(SyntaxType.FOR_KEYWORD);
            writer.WriteSpace();
            writer.WriteIdentifier(node.Variable.Name);
            writer.WritePunctuation(SyntaxType.EQUALS_TOKEN);
            writer.WriteSpace();
            node.LowerBound.WriteTo(writer);
            writer.WriteSpace();
            writer.WriteKeyword(SyntaxType.TO_KEYWORD);
            writer.WriteSpace();
            node.UpperBound.WriteTo(writer);
            writer.WriteLine();
            writer.WriteNestedStatement(node.Body);
        }

        private static void WriteGotoStatement(AbstractGotoStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword("goto");
            writer.WriteSpace();
            writer.WriteIdentifier(node.Label.Name);
            writer.WriteLine();
        }

        private static void WriteConditionalGotoStatement(AbstractConditionalGotoStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword("goto");
            writer.WriteSpace();
            writer.WriteIdentifier(node.Label.Name);
            writer.WriteSpace();
            writer.WriteKeyword(node.JumpIfTrue ? "if" : "unless");
            writer.WriteSpace();
            node.Condition.WriteTo(writer);
            writer.WriteLine();
        }

        private static void WriteLabelStatement(AbstractLabelStatement node, IndentedTextWriter writer)
        {
            bool unindent = writer.Indent > 0;
            if (unindent)
                writer.Indent--;
            writer.WritePunctuation(node.Label.Name);
            writer.WritePunctuation(SyntaxType.COLON_TOKEN);
            writer.WriteLine();
            if (unindent)
                writer.Indent++;
        }

        private static void WriteErrorExpression(AbstractErrorExpression _, IndentedTextWriter writer)
        {
            writer.WriteKeyword("???");
        }

        private static void WriteLiteralExpression(AbstractLiteralExpression node, IndentedTextWriter writer)
        {
            string value = node.Value.ToString()!;

            if (node.Type == TypeSymbol.Bool)
            {
                writer.WriteKeyword((bool)node.Value ? SyntaxType.TRUE_KEYWORD : SyntaxType.FALSE_KEYWORD);
                return;
            }

            if (node.Type == TypeSymbol.Int)
            {
                writer.WriteNumber(value);
                return;
            }

            if (node.Type == TypeSymbol.String)
            {
                value = "\"" + value.Replace("\"", "\"\"") + "\"";
                writer.WriteString(value);
                return;
            }

            throw new Exception($"Unexpected tpye {node.Type}");
        }

        private static void WriteCompoundAssignmentExpression(AbstractCompoundAssignmentExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.Variable.Name);
            writer.WriteSpace();
            writer.WritePunctuation(node.Operator.SyntaxType);
            writer.WritePunctuation(SyntaxType.EQUALS_TOKEN);
            writer.WriteSpace();
            node.Expression.WriteTo(writer);
        }
        private static void WriteUnaryExpression(AbstractUnaryExpression node, IndentedTextWriter writer)
        {
            int precedence = node.Operator.SyntaxType.GetUnaryOperatorPrecedence();
            string? textOp = node.Operator.SyntaxType.GetText();
            Debug.Assert(textOp is not null);

            writer.WritePunctuation(textOp);
            writer.WriteNestedExpression(precedence, node.Operand);
        }

        private static void WriteBinaryExpression(AbstractBinaryExpression node, IndentedTextWriter writer)
        {
            int precedence = node.Operator.SyntaxType.GetBinaryOperatorPrecedence();
            string? textOp = node.Operator.SyntaxType.GetText();
            Debug.Assert(textOp is not null);

            writer.WriteNestedExpression(precedence, node.Left);
            writer.WriteSpace();
            writer.WritePunctuation(textOp);
            writer.WriteSpace();
            writer.WriteNestedExpression(precedence, node.Right);
        }

        private static void WriteVariableExpression(AbstractVariableExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.Variable.Name);
        }

        private static void WriteAssignmentExpression(AbstractAssignmentExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.Variable.Name);
            writer.WriteSpace();
            writer.WritePunctuation(SyntaxType.EQUALS_TOKEN);
            writer.WriteSpace();
            node.Expression.WriteTo(writer);
        }

        private static void WriteCallExpression(AbstractCallExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.Function.Name);
            writer.WritePunctuation(SyntaxType.OPEN_PARENTHESIS_TOKEN);
            bool isFirst = true;
            foreach (AbstractExpression? argument in node.Arguments)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.WritePunctuation(SyntaxType.COMMA_TOKEN);
                    writer.WriteSpace();
                }
                argument.WriteTo(writer);
            }
            writer.WritePunctuation(SyntaxType.CLOSE_PARENTHESIS_TOKEN);
        }

        private static void WriteConversionExpression(AbstractConversionExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.Type.Name);
            writer.WritePunctuation(SyntaxType.OPEN_PARENTHESIS_TOKEN);
            node.Expression.WriteTo(writer);
            writer.WritePunctuation(SyntaxType.CLOSE_PARENTHESIS_TOKEN);
        }
    }
}
