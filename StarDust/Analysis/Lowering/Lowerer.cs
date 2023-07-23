using StarDust.Code.AST.ControlFlow;
using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Expressions.Expressions;
using StarDust.Code.AST.Nodes;
using StarDust.Code.AST.Processing;
using StarDust.Code.AST.Statements;
using StarDust.Code.Symbols;
using System.Collections.Immutable;
using static StarDust.Code.AST.Factory.AbstractNodeFactory;

namespace StarDust.Code.Lowering
{
    internal sealed class Lowerer
        : AbstractTreeRewriter
    {
        private int _labelCount;

        private Lowerer()
        {
        }

        private AbstractLabel GenerateLabel()
        {
            string name = $"Label{++_labelCount}";
            return new AbstractLabel(name);
        }

        public static AbstractBlockStatement Lower(FunctionSymbol function, AbstractStatement statement)
        {
            Lowerer lowerer = new();
            AbstractStatement result = lowerer.RewriteStatement(statement);
            return RemoveDeadCode(Flatten(function, result));
        }

        private static AbstractBlockStatement Flatten(FunctionSymbol function, AbstractStatement statement)
        {
            ImmutableArray<AbstractStatement>.Builder builder = ImmutableArray.CreateBuilder<AbstractStatement>();
            Stack<AbstractStatement> stack = new();
            stack.Push(statement);

            while (stack.Count > 0)
            {
                AbstractStatement current = stack.Pop();

                if (current is AbstractBlockStatement block)
                {
                    foreach (AbstractStatement? s in block.Statements.Reverse())
                        stack.Push(s);
                }
                else
                {
                    builder.Add(current);
                }
            }

            if (function.ReturnType == TypeSymbol.Void)
            {
                if (builder.Count == 0 || CanFallThrough(builder.Last()))
                {
                    builder.Add(new AbstractReturnStatement(statement.Syntax, null));
                }
            }

            return new AbstractBlockStatement(statement.Syntax, builder.ToImmutable());
        }

        private static bool CanFallThrough(AbstractStatement boundStatement)
        {
            return boundStatement.NodeType != AbstractNodeType.RETURN_STATEMENT &&
                   boundStatement.NodeType != AbstractNodeType.GOTO_STATEMENT;
        }

        private static AbstractBlockStatement RemoveDeadCode(AbstractBlockStatement node)
        {
            ControlFlowGraph controlFlow = ControlFlowGraph.Create(node);
            HashSet<AbstractStatement> reachableStatements = new(
                controlFlow.Blocks.SelectMany(b => b.Statements));

            ImmutableArray<AbstractStatement>.Builder builder = node.Statements.ToBuilder();
            for (int i = builder.Count - 1; i >= 0; i--)
            {
                if (!reachableStatements.Contains(builder[i]))
                    builder.RemoveAt(i);
            }

            return new AbstractBlockStatement(node.Syntax, builder.ToImmutable());
        }

        protected override AbstractStatement RewriteIfStatement(AbstractIfStatement node)
        {
            if (node.ElseStatement == null)
            {
                // if <condition>
                //      <then>
                //
                // ---->
                //
                // gotoFalse <condition> end
                // <then>
                // end:

                AbstractLabel endLabel = GenerateLabel();
                AbstractBlockStatement result = Block(
                    node.Syntax,
                    GotoFalse(node.Syntax, endLabel, node.Condition),
                    node.ThenStatement,
                    Label(node.Syntax, endLabel)
                );

                return RewriteStatement(result);
            }
            else
            {
                // if <condition>
                //      <then>
                // else
                //      <else>
                //
                // ---->
                //
                // gotoFalse <condition> else
                // <then>
                // goto end
                // else:
                // <else>
                // end:

                AbstractLabel elseLabel = GenerateLabel();
                AbstractLabel endLabel = GenerateLabel();
                AbstractBlockStatement result = Block(
                    node.Syntax,
                    GotoFalse(node.Syntax, elseLabel, node.Condition),
                    node.ThenStatement,
                    Goto(node.Syntax, endLabel),
                    Label(node.Syntax, elseLabel),
                    node.ElseStatement,
                    Label(node.Syntax, endLabel)
                );

                return RewriteStatement(result);
            }
        }

        protected override AbstractStatement RewriteWhileStatement(AbstractWhileStatement node)
        {
            // while <condition>
            //      <body>
            //
            // ----->
            //
            // goto continue
            // body:
            // <body>
            // continue:
            // gotoTrue <condition> body
            // break:

            AbstractLabel bodyLabel = GenerateLabel();
            AbstractBlockStatement result = Block(
                node.Syntax,
                Goto(node.Syntax, node.ContinueLabel),
                Label(node.Syntax, bodyLabel),
                node.Body,
                Label(node.Syntax, node.ContinueLabel),
                GotoTrue(node.Syntax, bodyLabel, node.Condition),
                Label(node.Syntax, node.BreakLabel)
            );

            return RewriteStatement(result);
        }

        protected override AbstractStatement RewriteDoWhileStatement(AbstractDoWhileStatement node)
        {
            // do
            //      <body>
            // while <condition>
            //
            // ----->
            //
            // body:
            // <body>
            // continue:
            // gotoTrue <condition> body
            // break:

            AbstractLabel bodyLabel = GenerateLabel();
            AbstractBlockStatement result = Block(
                node.Syntax,
                Label(node.Syntax, bodyLabel),
                node.Body,
                Label(node.Syntax, node.ContinueLabel),
                GotoTrue(node.Syntax, bodyLabel, node.Condition),
                Label(node.Syntax, node.BreakLabel)
            );

            return RewriteStatement(result);
        }

        protected override AbstractStatement RewriteForStatement(AbstractForStatement node)
        {
            // for <var> = <lower> to <upper>
            //      <body>
            //
            // ---->
            //
            // {
            //      var <var> = <lower>
            //      let upperBound = <upper>
            //      while (<var> <= upperBound)
            //      {
            //          <body>
            //          continue:
            //          <var> = <var> + 1
            //      }
            // }

            AbstractVariableDeclaration lowerBound = VariableDeclaration(node.Syntax, node.Variable, node.LowerBound);
            AbstractVariableDeclaration upperBound = ConstantDeclaration(node.Syntax, "upperBound", node.UpperBound);
            AbstractBlockStatement result = Block(
                node.Syntax,
                lowerBound,
                upperBound,
                While(node.Syntax,
                    LessOrEqual(
                        node.Syntax,
                        Variable(node.Syntax, lowerBound),
                        Variable(node.Syntax, upperBound)
                    ),
                    Block(
                        node.Syntax,
                        node.Body,
                        Label(node.Syntax, node.ContinueLabel),
                        Increment(
                            node.Syntax,
                            Variable(node.Syntax, lowerBound)
                    )
                ),
                node.BreakLabel,
                continueLabel: GenerateLabel())
            );


            return RewriteStatement(result);
        }

        protected override AbstractStatement RewriteConditionalGotoStatement(AbstractConditionalGotoStatement node)
        {
            if (node.Condition.ConstantValue != null)
            {
                bool condition = (bool)node.Condition.ConstantValue.Value;
                condition = node.JumpIfTrue ? condition : !condition;
                if (condition)
                    return RewriteStatement(Goto(node.Syntax, node.Label));
                else
                    return RewriteStatement(Nop(node.Syntax));
            }

            return base.RewriteConditionalGotoStatement(node);
        }

        protected override AbstractExpression RewriteCompoundAssignmentExpression(AbstractCompoundAssignmentExpression node)
        {
            AbstractCompoundAssignmentExpression newNode = (AbstractCompoundAssignmentExpression)base.RewriteCompoundAssignmentExpression(node);

            // a <op>= b
            //
            // ---->
            //
            // a = (a <op> b)

            AbstractAssignmentExpression result = Assignment(
                newNode.Syntax,
                newNode.Variable,
                Binary(
                    newNode.Syntax,
                    Variable(newNode.Syntax, newNode.Variable),
                    newNode.Operator,
                    newNode.Expression
                )
            );

            return result;
        }

        protected override AbstractStatement RewriteVariableDeclarationStatement(AbstractVariableDeclaration node)
        {
            AbstractStatement rewrittenNode = base.RewriteVariableDeclarationStatement(node);
            return new BoundSequencePointStatement(rewrittenNode.Syntax, rewrittenNode, rewrittenNode.Syntax.Location);
        }

        // Wrap expressions with sequence points
        protected override AbstractStatement RewriteExpressionStatement(AbstractExpressionStatement node)
        {
            AbstractStatement rewrittenNode = base.RewriteExpressionStatement(node);
            return new BoundSequencePointStatement(rewrittenNode.Syntax, rewrittenNode, rewrittenNode.Syntax.Location);
        }
    }
}