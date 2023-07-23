
using StarDust.Code.AST;
using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Nodes;
using StarDust.Code.AST.Statements;
using StarDust.Code.Symbols;
using System.Diagnostics;

namespace StarDust.Code
{
    internal sealed class Evaluator
    {
        private readonly AbstractProgram Program;

        // Local and Global variables
        private readonly Dictionary<VariableSymbol, object> Globals;
        private readonly Stack<Dictionary<VariableSymbol, object>> Locals = new();
        private readonly Dictionary<FunctionSymbol, AbstractBlockStatement> Functions = new();
        private object? LastValue;
        public Evaluator(AbstractProgram program, Dictionary<VariableSymbol, object> variables)
        {
            Program = program;
            Globals = variables;
            Locals.Push(new());

            AbstractProgram? p = program;
            while (p != null)
            {
                foreach ((FunctionSymbol function, AbstractBlockStatement body) in p.Functions)
                {
                    Functions.Add(function, body);
                }

                p = p.Previous;
            }
        }

        public object? Evaluate()
        {
            FunctionSymbol? function = Program.MainFunction ?? Program.ScriptFunction;
            if (function is null)
                return null;

            AbstractBlockStatement? body = Functions[function];
            return EvaluateStatement(body);
        }

        private object? EvaluateStatement(AbstractBlockStatement statement)
        {
            Dictionary<AbstractLabel, int> labelToIndex = new();
            for (int i = 0; i < statement.Statements.Length; i++)
            {
                if (statement.Statements[i] is AbstractLabelStatement l)
                {
                    labelToIndex.Add(l.Label, i + 1);
                }
            }

            AbstractStatement[] statements = statement.Statements.ToArray();
            for (int i = 0; i < statements.Length; i++)
            {
                if (statements[i] is BoundSequencePointStatement s)
                    statements[i] = s.Statement;
            }


            int index = 0;
            while (index < statements.Length)
            {
                AbstractStatement s = statements[index];
                switch (s.NodeType)
                {
                    case AbstractNodeType.NOP_STATEMENT:
                        index++;
                        break;
                    case AbstractNodeType.VARIABLE_DECLARATION_STATEMENT:
                        EvaluateVariableDeclaration((AbstractVariableDeclaration)s);
                        index++;
                        break;
                    case AbstractNodeType.EXPRESSION_STATEMENT:
                        EvaluateExpressionStatement((AbstractExpressionStatement)s);
                        index++;
                        break;
                    case AbstractNodeType.GOTO_STATEMENT:
                        AbstractGotoStatement gs = (AbstractGotoStatement)s;
                        index = labelToIndex[gs.Label];
                        break;
                    case AbstractNodeType.CONDITIONAL_GOTO_STATEMENT:
                        AbstractConditionalGotoStatement? cgs = (AbstractConditionalGotoStatement)s;
                        bool condition = (bool)EvaluateExpression(cgs.Condition)!;
                        if (condition == cgs.JumpIfTrue)
                            index = labelToIndex[cgs.Label];
                        else
                            index++;
                        break;
                    case AbstractNodeType.LABEL_STATEMENT:
                        index++;
                        break;
                    case AbstractNodeType.RETURN_STATEMENT:
                        {
                            AbstractReturnStatement? rs = (AbstractReturnStatement)s;
                            LastValue = rs.Expression == null ? null : EvaluateExpression(rs.Expression);
                            return LastValue;
                        }
                    default:
                        throw new Exception($"Unexpected node <{s.NodeType}>");
                }
            }

            return LastValue;
        }

        private object? EvaluateExpression(AbstractExpression node)
        {
            if (node.ConstantValue != null)
                return EvaluateConstantExpression(node);

            return node.NodeType switch
            {
                //BoundNodeType.LITERAL_EXPRESSION => EvaluateLiteralExpression((BoundLiteralExpression)Node),
                AbstractNodeType.VARIABLE_EXPRESSION => EvaluateVariableExpression((AbstractVariableExpression)node),
                AbstractNodeType.ASSIGNMENT_EXPRESSION => EvaluateAssignmentExpression((AbstractAssignmentExpression)node),
                AbstractNodeType.BINARY_EXPRESSION => EvaluateBinaryExpression((AbstractBinaryExpression)node),
                AbstractNodeType.UNARY_EXPRESSION => EvaluateUnaryExpression((AbstractUnaryExpression)node),
                AbstractNodeType.CALL_EXPRESSION => EvaluateCallExpression((AbstractCallExpression)node),
                AbstractNodeType.CONVERSION_EXPRESSION => EvaluateConversionExpression((AbstractConversionExpression)node),
                _ => throw new Exception($"Unexpected node <{node.NodeType}>"),
            };
        }

        private object? EvaluateConversionExpression(AbstractConversionExpression node)
        {
            object? value = EvaluateExpression(node.Expression);
            if (node.Type == TypeSymbol.Any)
                return value;

            if (node.Type == TypeSymbol.Bool)
                return Convert.ToBoolean(value);

            if (node.Type == TypeSymbol.Int)
                return Convert.ToInt32(value);

            if (node.Type == TypeSymbol.String)
                return Convert.ToString(value);

            throw new Exception($"Unexpected type {node.Type}");
        }

        private object EvaluateUnaryExpression(AbstractUnaryExpression node)
        {
            object? operand = EvaluateExpression(node.Operand);
            Debug.Assert(operand is not null);
            return node.Operator.UnaryType switch
            {
                AbstractUnaryType.NEGATION => -(int)operand,
                AbstractUnaryType.IDENTITY => (int)operand,
                AbstractUnaryType.LOGICAL_NEGATION => !(bool)operand,
                AbstractUnaryType.ONES_COMPLEMENT => ~(int)operand,
                _ => throw new Exception($"Unexpected unary operator <{node.Operator}>"),
            };
        }

        private object EvaluateBinaryExpression(AbstractBinaryExpression node)
        {
            object? lValue = EvaluateExpression(node.Left);
            object? rValue = EvaluateExpression(node.Right);
            Debug.Assert(lValue is not null && rValue is not null);

            return node.Operator.BinaryType switch
            {
                AbstractBinaryType.ADDITION => EvaluateAddition(node, lValue, rValue),
                AbstractBinaryType.SUBTRACTION => (int)lValue - (int)rValue,
                AbstractBinaryType.MULTIPLICATION => (int)lValue * (int)rValue,
                AbstractBinaryType.DIVISION => (int)lValue / (int)rValue,

                AbstractBinaryType.BITWISE_AND => EvaluateBitwiseAnd(node, lValue, rValue),
                AbstractBinaryType.BITWISE_OR => EvaluateBitwiseOr(node, lValue, rValue),
                AbstractBinaryType.BITWISE_XOR => EvaluateBitwiseXOR(node, lValue, rValue),

                AbstractBinaryType.LOGICAL_AND => (bool)lValue && (bool)rValue,
                AbstractBinaryType.LOGICAL_OR => (bool)lValue || (bool)rValue,
                AbstractBinaryType.EQUALS => Equals(lValue, rValue),
                AbstractBinaryType.NOT_EQUALS => !Equals(lValue, rValue),
                AbstractBinaryType.LESS_THAN => (int)lValue < (int)rValue,
                AbstractBinaryType.LESS_THAN_OR_EQUAL => (int)lValue <= (int)rValue,
                AbstractBinaryType.GREATER_THAN => (int)lValue > (int)rValue,
                AbstractBinaryType.GREATER_THAN_OR_EQUAL => (int)lValue >= (int)rValue,
                _ => throw new Exception($"Unexpected binary operator <{node.Operator}>"),
            };
        }

        #region Evaluators
        private void EvaluateVariableDeclaration(AbstractVariableDeclaration node)
        {
            object? value = EvaluateExpression(node.Initializer);
            Debug.Assert(value is not null);

            LastValue = value;
            Assign(node.Variable, value);
        }

        private void EvaluateExpressionStatement(AbstractExpressionStatement node)
        {
            LastValue = EvaluateExpression(node.Expression);
        }

        private object? EvaluateCallExpression(AbstractCallExpression node)
        {
            //if (node.Function == BuiltinFunction.Input)
            //{
            //    return Console.ReadLine();
            //}
            //else if (node.Function == BuiltinFunction.Print)
            //{
            //    object? value = EvaluateExpression(node.Arguments[0]);
            //    Console.WriteLine(value);
            //    return null;
            //}
            //else if (node.Function == BuiltinFunction.Random)
            //{
            //    int max = (int)EvaluateExpression(node.Arguments[0])!;
            //    if (Random is null)
            //        Random = new Random();
            //
            //    return Random.Next(max);
            //}
            //else
            {
                Dictionary<VariableSymbol, object> locals = new();

                for (int i = 0; i < node.Arguments.Length; i++)
                {
                    ParameterSymbol? parameter = node.Function.Parameters[i];
                    object? value = EvaluateExpression(node.Arguments[i]);
                    Debug.Assert(value is not null);
                    locals.Add(parameter, value);
                }
                Locals.Push(locals);
                AbstractBlockStatement statement = Functions[node.Function];
                object? result = EvaluateStatement(statement);
                Locals.Pop();
                return result;
            }
        }

        private object EvaluateVariableExpression(AbstractVariableExpression v)
        {
            if (v.Variable.SymbolType != SymbolType.GLOBAL_VARIABLE)
            {
                Dictionary<VariableSymbol, object>? locals = Locals.Peek();
                return locals[v.Variable];
            }

            return Globals[v.Variable];
        }

        private object EvaluateAssignmentExpression(AbstractAssignmentExpression a)
        {
            object? value = EvaluateExpression(a.Expression);
            Debug.Assert(value is not null);

            Assign(a.Variable, value);
            return value;
        }

        private static object EvaluateAddition(AbstractBinaryExpression n, object lValue, object rValue)
        {
            if (n.Type == TypeSymbol.Int)
                return (int)lValue + (int)rValue;
            //if (bNode.Type == TypeSymbol.String)
            return (string)lValue + (string)rValue;
        }

        private static object EvaluateBitwiseOr(AbstractBinaryExpression n, object lValue, object rValue)
        {
            if (n.Type == TypeSymbol.Int)
                return (int)lValue | (int)rValue;

            return (bool)lValue || (bool)rValue;
        }

        private static object EvaluateBitwiseXOR(AbstractBinaryExpression n, object lValue, object rValue)
        {
            if (n.Type == TypeSymbol.Int)
                return (int)lValue ^ (int)rValue;

            return (bool)lValue ^ (bool)rValue;
        }

        private static object EvaluateBitwiseAnd(AbstractBinaryExpression n, object lValue, object rValue)
        {
            if (n.Type == TypeSymbol.Int)
                return (int)lValue & (int)rValue;

            return (bool)lValue && (bool)rValue;
        }

        private static object EvaluateConstantExpression(AbstractExpression n)
        {
            Debug.Assert(n.ConstantValue is not null);
            return n.ConstantValue.Value;
        }

        #endregion

        private void Assign(VariableSymbol variableSymbol, object value)
        {
            if (variableSymbol.SymbolType != SymbolType.GLOBAL_VARIABLE)
            {
                Dictionary<VariableSymbol, object>? locals = Locals.Peek();
                locals[variableSymbol] = value;
                return;
            }

            Globals[variableSymbol] = value;
        }
    }
}
