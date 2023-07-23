using StarDust.Code;
using StarDust.Code.Evaluation;
using StarDust.Code.Extensions;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;
using StarDust.Code.Text;
using Xunit;

namespace StarDust.Test
{
    public class EvaluationTest
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("+1", 1)]
        [InlineData("-1", -1)]
        [InlineData("~1", -2)]
        [InlineData("1 + 2", 3)]
        [InlineData("1 - 2", -1)]
        [InlineData("1 * 2", 2)]
        [InlineData("8 / 2", 4)]
        [InlineData("(10 + 1) * 2", 22)]

        // Conditionals
        [InlineData("2 == 2", true)]
        [InlineData("2 == 1", false)]
        [InlineData("2 != 2", false)]
        [InlineData("2 != 1", true)]

        [InlineData("3 < 4", true)]
        [InlineData("5 < 4", false)]
        [InlineData("4 <= 4", true)]
        [InlineData("4 <= 5", true)]
        [InlineData("5 <= 4", false)]

        [InlineData("4 > 3", true)]
        [InlineData("4 > 5", false)]
        [InlineData("4 >= 4", true)]
        [InlineData("5 >= 4", true)]
        [InlineData("4 >= 5", false)]

        // Bitwise
        [InlineData("1 | 2", 3)]
        [InlineData("1 | 0", 1)]
        [InlineData("1 & 2", 0)]
        [InlineData("1 & 0", 0)]
        [InlineData("1 ^ 0", 1)]
        [InlineData("0 ^ 1", 1)]
        [InlineData("1 ^ 3", 2)]

        // Bools    
        [InlineData("true != false", true)]
        [InlineData("false != false", false)]
        [InlineData("true == false", false)]
        [InlineData("true && true", true)]
        [InlineData("false || false", false)]

        [InlineData("false | false", false)]
        [InlineData("false | true", true)]
        [InlineData("true  | false", true)]
        [InlineData("true  | true", true)]

        [InlineData("false ^ false", false)]
        [InlineData("false ^ true", true)]
        [InlineData("true  ^ false", true)]
        [InlineData("true  ^ true", false)]

        [InlineData("false & false", false)]
        [InlineData("false & true", false)]
        [InlineData("true  & false", false)]
        [InlineData("true  & true", true)]

        [InlineData("false == false", true)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!false", true)]
        [InlineData("!true", false)]

        // Variables
        [InlineData("var a = 10 return a", 10)]
        [InlineData("\"test\"", "test")]
        [InlineData("\"te\"\"st\"", "te\"st")]
        [InlineData("\"test\" == \"test\"", true)]
        [InlineData("\"test\" == \"hello\"", false)]
        [InlineData("\"test\" != \"test\"", false)]
        [InlineData("\"test\" != \"hello\"", true)]

        [InlineData("{ var a = 10; return (a * a); }", 100)]
        [InlineData("{ var a = 0; return (a = 10) * a;}", 100)]
        [InlineData("{ var a = 0; if (a == 0 a = 10) return a;}", 10)]
        [InlineData("{ var a = 0; if (a == 4 a = 10) return a;}", 0)]

        [InlineData("{ var a = 0 if a == 0 a = 10 else a = 5 return a}", 10)]
        [InlineData("{ var a = 0 if a == 4 a = 10 else a = 5 return a}", 5)]
        [InlineData("{ var i = 10 var result = 0 while i > 0 {result = result + i i = i - 1} return result}", 55)]
        [InlineData("{ var result = 0 for i = 1 to 10 {result = result + i} return result}", 55)]
        [InlineData("{ var a = 10 for i = 1 to (a = a - 1) {} return a }", 9)]
        [InlineData("{ var a = 0 do a = a + 1 while a < 10 return a }", 10)]

        [InlineData("{ var i = 0 while i < 5 { i = i + 1 if i == 5 continue } return i}", 5)]
        [InlineData("{ var i = 0 do { i = i + 1 if i == 5 continue } while i < 5 return i}", 5)]
        [InlineData("{ var a = 0 for i = 1 to 10 {a = a + i if a == 10 continue} return a}", 55)]

        [InlineData("\"test\" + \"abc\"", "testabc")]

        [InlineData("{ var any a = 0  var any b = \"b\" return a == b }", false)]
        [InlineData("{ var any a = 0  var any b = \"b\" return a != b }", true)]
        [InlineData("{ var any a = 0  var any b = 0 return a == b }", true)]
        [InlineData("{ var any a = 0  var any b = 0 return a != b }", false)]

        [InlineData("{ var a = 1 a += (2 + 3) return a }", 6)]
        [InlineData("{ var a = 1 a -= (2 + 3) return a }", -4)]
        [InlineData("{ var a = 1 a *= (2 + 3) return a }", 5)]
        [InlineData("{ var a = 1 a /= (2 + 3) return a }", 0)]
        [InlineData("{ var a = true a &= (false) return a }", false)]
        [InlineData("{ var a = true a |= (false) return a }", true)]
        [InlineData("{ var a = true a ^= (true) return a }", false)]
        [InlineData("{ var a = 1; a |= 0; return a; }", 1)]
        [InlineData("{ var a = 1; a &= 3; return a; }", 1)]
        [InlineData("{ var a = 1; a &= 0; return a; }", 0)]
        [InlineData("{ var a = 1; a ^= 0; return a; }", 1)]
        [InlineData("{ var a = 1; var b = 2; var c = 3; a += b; += c; return a; }", 6)]
        [InlineData("{ var a = 1; var b = 2; var c = 3; a += b; += c; return b; }", 5)]
        public void Evaluator_Evaluate(string Text, object ExpectedValue) => AssertValue(Text, ExpectedValue);

        [Fact]
        public void Evaluator_Void_Function_Should_Not_Return_Value()
        {
            const string text = @"
                fn test()
                {
                    return [1]
                }
            ";

            const string diagnostics = @"
                Function <test> does not have a return type. <return> keyword cannot be followed by an expression
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Function_With_ReturnValue_Should_Not_Return_Void()
        {
            const string text = @"
                fn int test()
                {
                    [return]
                }
            ";

            const string diagnostics = @"
                Function <test> expects a return type of <int> but no expression was found after <return> keyword.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Not_All_Code_Paths_Return_Value()
        {
            const string text = @"
                fn bool [test](int n)
                {
                    if (n > 10)
                       return true
                }
            ";

            const string diagnostics = @"
                Not all code paths return a value
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Expression_Must_Have_Value()
        {
            const string text = @"
                fn test(int n)
                {
                    return
                }
                const value = [test(100)]
            ";

            const string diagnostics = @"
                Expression must have a value.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_IfStatement_Reports_NotReachableCode_Warning()
        {
            string text = @"
                fn test()
                {
                    const x = 4 * 3
                    if x > 12
                    {
                        [print](""x"")
                    }
                    else
                    {
                        print(""x"")
                    }
                }
            ";

            string diagnostics = @"
                Unreachable code detected.
            ";
            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_ElseStatement_Reports_NotReachableCode_Warning()
        {
            string text = @"
                fn int test()
                {
                    if true
                    {
                        return 1;
                    }
                    else
                    {
                        [return] 0;
                    }
                }
            ";

            string diagnostics = @"
                Unreachable code detected.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_WhileStatement_Reports_NotReachableCode_Warning()
        {
            string text = @"
                fn test()
                {
                    while false
                    {
                        [continue]
                    }
                }
            ";

            string diagnostics = @"
                Unreachable code detected.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Theory]
        [InlineData("[break]", "break")]
        [InlineData("[continue]", "continue")]
        public void Evaluator_Invalid_Break_Or_Continue(string text, string keyword)
        {
            string? diagnostics = $@"
                The keyword <{keyword}> can only be used inside loops.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Script_Return()
        {
            const string text = @"
                return
            "
;
            // Return is allowed in scripts
            const string diagnostics = @"
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Parameter_Already_Declared()
        {
            const string text = @"
                fn int sum(int a, int b, [int a])
                {
                    return a + b + c
                }
            ";

            const string diagnostics = @"
                Parameter <a> is already declared.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Wrong_Argument_Type()
        {
            const string text = @"
                fn bool test(int n)
                {
                    return n > 10
                }
                const testValue = ""string""
                test([testValue])
            ";

            const string diagnostics = @"
               Cannot convert type of <string> to <int>. An explicit conversion exists (are you missing a cast?).
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Bad_Type()
        {
            const string text = @"
                fn test([invalidtype] n)
                {
                }
            ";

            const string diagnostics = @"
                Type <invalidtype> does not exist.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_VariableDeclaration_Reports_Redecleration()
        {
            const string text = @"
                {
                    var x = 0
                    var y = 1
                    {
                        var x = 10
                    }
                    var [x] = 5
                }
                ";
            const string report = "<x> is already declared.";

            AssertDiagnostics(text, report);
        }

        [Fact]
        public void Evaluator_InvokeFunctionArguments_Missing()
        {
            const string text = @"
                print([)]
            ";

            const string diagnostics = @"
                Function <print> requires 1 arguments but was given 0.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_InvokeFunctionArguments_Exceeding()
        {
            const string text = @"
                print(""Hello""[, "" "", "" world!""])
            ";

            const string diagnostics = @"
                Function <print> requires 1 arguments but was given 3.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_BlockStatement_NoInfiniteLoop()
        {
            const string text = @"
                {
                [)][]
                ";

            const string report = @"
                        Unexpected token:<CLOSE_PARENTHESIS_TOKEN>, expected <IDENTIFIER_TOKEN>.
                        Unexpected token:<END_OF_FILE_TOKEN>, expected <CLOSE_BRACE_TOKEN>.
                          ";

            AssertDiagnostics(text, report);
        }

        [Fact]
        public void Evaluator_InvokeFunctionArguments_NoInfiniteLoop()
        {
            const string text = @"
                print(""Hi""[[=]][)]
            ";

            const string diagnostics = @"
                Unexpected token:<EQUALS_TOKEN>, expected <CLOSE_PARENTHESIS_TOKEN>.
                Unexpected token:<EQUALS_TOKEN>, expected <IDENTIFIER_TOKEN>.
                Unexpected token:<CLOSE_PARENTHESIS_TOKEN>, expected <IDENTIFIER_TOKEN>.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_FunctionParameters_NoInfiniteLoop()
        {
            const string text = @"
                fn hi(string name[[[=]]][)]
                {
                    print(""Hi "" + name + ""!"" )
                }[]
            ";

            const string diagnostics = @"
                Unexpected token:<EQUALS_TOKEN>, expected <CLOSE_PARENTHESIS_TOKEN>.
                Unexpected token:<EQUALS_TOKEN>, expected <OPEN_BRACE_TOKEN>.
                Unexpected token:<EQUALS_TOKEN>, expected <IDENTIFIER_TOKEN>.
                Unexpected token:<CLOSE_PARENTHESIS_TOKEN>, expected <IDENTIFIER_TOKEN>.
                Unexpected token:<END_OF_FILE_TOKEN>, expected <CLOSE_BRACE_TOKEN>.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_NameExpression_Reports_NoErrorForInsertedToken()
        {
            const string text = "1 + []";

            const string report = "Unexpected token:<END_OF_FILE_TOKEN>, expected <IDENTIFIER_TOKEN>.";

            AssertDiagnostics(text, report);
        }

        [Fact]
        public void Evaluator_Name_Reports_Undefined()
        {
            const string text = "[x] * 10";
            const string report = "Variable name does not exist <x>.";

            AssertDiagnostics(text, report);
        }

        [Fact]
        public void Evaluator_Assignment_Reports_Undefined()
        {
            const string text = "[x] = 10";
            const string report = "Variable name does not exist <x>.";

            AssertDiagnostics(text, report);
        }

        [Fact]
        public void Evaluator_AssignmentExpression_Reports_NotAVariable()
        {
            const string text = "[print] = 42;";

            const string diagnostics = @"
                <print> is not a variable.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_CompoundExpression_Assignment_NonDefinedVariable_Reports_Undefined()
        {
            const string text = "[x] += 10";

            const string diagnostics = "Variable name does not exist <x>.";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Assignment_Reports_CannotAssign()
        {
            const string text = @"
                        {
                            const x = 10;
                            x [=] 0;
                            return x;
                        }
                        ";
            const string report = "Cannot reassign value of read-only variable <x>.";

            AssertDiagnostics(text, report);
        }

        [Fact]
        public void Evaluator_CompoundDeclarationExpression_Reports_CannotAssign()
        {
            string text = @"
                {
                    const x = 10;
                    x [+=] 1;
                }
            ";

            string diagnostics = @"
                Cannot reassign value of read-only variable <x>.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Assignment_Reports_CannotConvert()
        {
            const string text = @"
                        {
                            var x = 10;
                            x = [true];
                        }
                        ";

            const string report = "Cannot convert type of <bool> to <int>.";

            AssertDiagnostics(text, report);
        }

        [Fact]
        public void Evaluator_Unary_Reports_Undefined()
        {
            const string text = "[+]true";

            const string report = "Unary operator <+> is not defined for type <bool>.";

            AssertDiagnostics(text, report);
        }

        [Fact]
        public void Evaluator_Binary_Reports_Undefined()
        {
            const string text = "10 [+] true";

            const string report = "Binary operator <+> is not defined for types <int> and <bool>.";

            AssertDiagnostics(text, report);
        }

        [Fact]
        public void Evaluator_CompoundExpression_Reports_Undefined()
        {
            string? text = @"var x = 10 ;
                         x [+=] false;";

            string? diagnostics = @"
                Binary operator <+=> is not defined for types <int> and <bool>.
            ";

            AssertDiagnostics(text, diagnostics);
        }



        [Fact]
        public void Evaluator_IfStatement_Reports_CannotConvert()
        {
            const string text = @"
                        {
                            var x = 0
                            if [10]
                                x = 10
                        
                        }
                        ";

            const string report = "Cannot convert type of <int> to <bool>.";

            AssertDiagnostics(text, report);
        }

        [Fact]
        public void Evaluator_WhileStatement_Reports_CannotConvert()
        {
            const string text = @"
                        {
                            var x = 0
                            while [10]
                                x = 10
                        
                        }
                        ";

            const string report = "Cannot convert type of <int> to <bool>.";

            AssertDiagnostics(text, report);
        }

        [Fact]
        public void Evaluator_DoWhileStatement_Reports_CannotConvert()
        {
            const string text = @"
                        {
                            var x = 0;
                            do
                                x = 10
                            while [10];
                        }
                        ";

            const string report = "Cannot convert type of <int> to <bool>.";

            AssertDiagnostics(text, report);
        }

        [Fact]
        public void Evaluator_CallExpression_Reports_Undefined()
        {
            const string text = "[foo](42);";

            const string diagnostics = @"
                Function <foo> does not exist.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_CallExpression_Reports_NotAFunction()
        {
            const string text = @"
                {
                    const foo = 42;
                    [foo](42);
                }
            ";

            const string diagnostics = @"
                <foo> is not a function.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Variables_Can_Shadow_Functions()
        {
            const string text = @"
                {
                    const print = 42
                    [print](""test"")
                }
            ";

            const string diagnostics = @"
                <print> is not a function.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_ForStatement_Reports_CannotConvert_LowerBound()
        {
            const string text = @"
                        {
                            var result = 0
                            for i = [false] to 10
                                result = result + i
                        
                        }
                        ";

            const string report = "Cannot convert type of <bool> to <int>.";

            AssertDiagnostics(text, report);
        }

        [Fact]
        public void Evaluator_ForStatement_Reports_CannotConvert_UpperBound()
        {
            const string text = @"
                        {
                            var result = 0
                            for i = 1 to [false]
                                result = result + i
                        
                        }
                        ";

            const string report = "Cannot convert type of <bool> to <int>.";

            AssertDiagnostics(text, report);
        }

        private static void AssertValue(string Text, object expectedVal)
        {
            ConcreteSyntaxTree tree = ConcreteSyntaxTree.Parse(Text);
            Compilation comp = Compilation.CreateScript(null, tree);
            Dictionary<VariableSymbol, object> Vars = new();

            EvaluationResult result = comp.Evaluate(Vars);

            Assert.False(result.Report.HasErrors());
            Assert.Equal(expectedVal, result.Value);
        }

        private void AssertDiagnostics(string text, string reportText)
        {
            AnnotatedText? annotatedText = AnnotatedText.Parse(text);
            ConcreteSyntaxTree? syntaxTree = ConcreteSyntaxTree.Parse(annotatedText.Text);
            Compilation compilation = Compilation.CreateScript(null, syntaxTree);
            EvaluationResult? result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

            string[]? expectedReport = AnnotatedText.UnindentLines(reportText);
            if (annotatedText.Spans.Length != expectedReport.Length)
            {
                throw new System.Exception("ERROR :: Must mark as many spans as there are expected reports");
            }

            System.Collections.Immutable.ImmutableArray<Report> report = result.Report;
            Assert.Equal(expectedReport.Length, report.Length);
            for (int i = 0; i < expectedReport.Length; i++)
            {
                string? expectedMessage = expectedReport[i];
                string? actualMessage = report[i].Message;

                TextSpan expectedSpan = annotatedText.Spans[i];
                TextSpan actualSpan = report[i].Location.Span;

                Assert.Equal(expectedMessage, actualMessage);
                Assert.Equal(expectedSpan, actualSpan);
            }
        }
    }
}
