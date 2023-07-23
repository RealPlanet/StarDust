using StarDust.Code;
using StarDust.Code.Extensions;
using StarDust.Code.Syntax;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace StarDust.Test
{
    public class ParserTests
    {
        private static Expression ParseExpression(string text)
        {
            ConcreteSyntaxTree syntaxTree = ConcreteSyntaxTree.Parse(text);
            CompilationUnit root = syntaxTree.Root;
            Member member = Assert.Single(root.Members);
            GlobalStatement globalStatement = Assert.IsType<GlobalStatement>(member);
            return Assert.IsType<ExpressionStatement>(globalStatement.Statement).Expression;
        }

        [Theory]
        [MemberData(nameof(GetBinaryOperatorPairsData))]
        public void Parser_BinaryExpression_HonorsPrecedences(SyntaxType op1, SyntaxType op2)
        {
            int op1Precedence = op1.GetBinaryOperatorPrecedence();
            int op2Precedence = op2.GetBinaryOperatorPrecedence();

            string? op1Text = op1.GetText();
            string? op2Text = op2.GetText();

            string text = $"a {op1Text} b {op2Text} c";
            Expression? expression = ParseExpression(text);

            Debug.Assert(op1Text != null);
            Debug.Assert(op2Text != null);

            if (op1Precedence >= op2Precedence)
            {
                //       op2
                //      /   \
                //   op1    c
                //  /  \
                // a    b

                using (AssertingEnum? Enum = new(expression))
                {
                    Enum.AssertNode(SyntaxType.BINARY_EXPRESSION_NODE);
                    Enum.AssertNode(SyntaxType.BINARY_EXPRESSION_NODE);
                    Enum.AssertNode(SyntaxType.NAME_EXPRESSION_NODE);
                    Enum.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "a");
                    Enum.AssertToken(op1, op1Text);

                    Enum.AssertNode(SyntaxType.NAME_EXPRESSION_NODE);
                    Enum.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "b");
                    Enum.AssertToken(op2, op2Text);

                    Enum.AssertNode(SyntaxType.NAME_EXPRESSION_NODE);
                    Enum.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "c");
                }
            }
            else
            {
                //       op1
                //      /   \
                //      a    op2
                //           /  \
                //          b    c

                using (AssertingEnum? Enum = new(expression))
                {
                    Enum.AssertNode(SyntaxType.BINARY_EXPRESSION_NODE);
                    Enum.AssertNode(SyntaxType.NAME_EXPRESSION_NODE);
                    Enum.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "a");
                    Enum.AssertToken(op1, op1Text);
                    Enum.AssertNode(SyntaxType.BINARY_EXPRESSION_NODE);
                    Enum.AssertNode(SyntaxType.NAME_EXPRESSION_NODE);
                    Enum.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "b");
                    Enum.AssertToken(op2, op2Text);
                    Enum.AssertNode(SyntaxType.NAME_EXPRESSION_NODE);
                    Enum.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "c");
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetUnaryOperatorPairsData))]
        public void Parser_UnaryExpression_HonorsPrecedences(SyntaxType UnaryType, SyntaxType BinaryType)
        {
            int unaryPrecedence = UnaryType.GetUnaryOperatorPrecedence();
            int binaryPrecedence = BinaryType.GetBinaryOperatorPrecedence();

            string? unaryText = UnaryType.GetText();
            string? binaryText = BinaryType.GetText();

            string text = $"{unaryText} a {binaryText} b";
            Expression? expression = ParseExpression(text);

            Debug.Assert(unaryText != null);
            Debug.Assert(binaryText != null);

            if (unaryPrecedence >= binaryPrecedence)
            {
                //   binary
                //   /    \
                // unary   b
                //  | 
                //  a 

                using (AssertingEnum? Enum = new(expression))
                {
                    Enum.AssertNode(SyntaxType.BINARY_EXPRESSION_NODE);
                    Enum.AssertNode(SyntaxType.UNARY_EXPRESSION_NODE);
                    Enum.AssertToken(UnaryType, unaryText);
                    Enum.AssertNode(SyntaxType.NAME_EXPRESSION_NODE);
                    Enum.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "a");
                    Enum.AssertToken(BinaryType, binaryText);
                    Enum.AssertNode(SyntaxType.NAME_EXPRESSION_NODE);
                    Enum.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "b");
                }
            }
            else
            {
                //   unary
                //     |
                //   binary
                //  /     \
                // a       b

                using (AssertingEnum? Enum = new(expression))
                {
                    Enum.AssertNode(SyntaxType.UNARY_EXPRESSION_NODE);
                    Enum.AssertToken(UnaryType, unaryText);
                    Enum.AssertNode(SyntaxType.BINARY_EXPRESSION_NODE);
                    Enum.AssertNode(SyntaxType.NAME_EXPRESSION_NODE);
                    Enum.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "a");
                    Enum.AssertToken(BinaryType, binaryText);
                    Enum.AssertNode(SyntaxType.NAME_EXPRESSION_NODE);
                    Enum.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "b");
                }
            }
        }

        public static IEnumerable<object[]> GetBinaryOperatorPairsData()
        {
            foreach (SyntaxType op1 in SyntaxExtensions.GetBinaryOperatorTypes())
            {
                foreach (SyntaxType op2 in SyntaxExtensions.GetBinaryOperatorTypes())
                {
                    yield return new object[] { op1, op2 };
                }
            }
        }

        public static IEnumerable<object[]> GetUnaryOperatorPairsData()
        {
            foreach (SyntaxType unary in SyntaxExtensions.GetUnaryOperatorTypes())
            {
                foreach (SyntaxType binary in SyntaxExtensions.GetBinaryOperatorTypes())
                {
                    yield return new object[] { unary, binary };
                }
            }
        }
    }
}