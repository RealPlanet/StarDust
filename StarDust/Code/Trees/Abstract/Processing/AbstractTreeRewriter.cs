using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Expressions.Expressions;
using StarDust.Code.AST.Nodes;
using StarDust.Code.AST.Statements;
using System.Collections.Immutable;

namespace StarDust.Code.AST.Processing
{
    internal abstract class AbstractTreeRewriter
    {
        public virtual AbstractStatement RewriteStatement(AbstractStatement node)
        {
            return node.NodeType switch
            {
                AbstractNodeType.SEQUENCE_POINT_STATEMENT => RewriteSequencePointStatement((BoundSequencePointStatement)node),
                AbstractNodeType.BLOCK_STATEMENT => RewriteBlockStatement((AbstractBlockStatement)node),
                AbstractNodeType.NOP_STATEMENT => RewriteNopStatement((AbstractNopStatement)node),
                AbstractNodeType.EXPRESSION_STATEMENT => RewriteExpressionStatement((AbstractExpressionStatement)node),
                AbstractNodeType.VARIABLE_DECLARATION_STATEMENT => RewriteVariableDeclarationStatement((AbstractVariableDeclaration)node),
                AbstractNodeType.IF_STATEMENT => RewriteIfStatement((AbstractIfStatement)node),
                AbstractNodeType.WHILE_STATEMENT => RewriteWhileStatement((AbstractWhileStatement)node),
                AbstractNodeType.DO_WHILE_STATEMENT => RewriteDoWhileStatement((AbstractDoWhileStatement)node),
                AbstractNodeType.FOR_STATEMENT => RewriteForStatement((AbstractForStatement)node),
                AbstractNodeType.LABEL_STATEMENT => RewriteLabelStatement((AbstractLabelStatement)node),
                AbstractNodeType.GOTO_STATEMENT => RewriteGotoStatement((AbstractGotoStatement)node),
                AbstractNodeType.CONDITIONAL_GOTO_STATEMENT => RewriteConditionalGotoStatement((AbstractConditionalGotoStatement)node),
                AbstractNodeType.RETURN_STATEMENT => RewriteReturnStatement((AbstractReturnStatement)node),
                _ => throw new Exception($"Unexpected node: {node.NodeType}"),
            };
        }

        private AbstractStatement RewriteSequencePointStatement(BoundSequencePointStatement node)
        {
            AbstractStatement statement = RewriteStatement(node.Statement);
            if (statement == node.Statement)
                return node;
            return new BoundSequencePointStatement(node.Syntax, statement, node.Location);
        }

        public virtual AbstractStatement RewriteNopStatement(AbstractNopStatement node)
        {
            return node;
        }

        public virtual AbstractExpression RewriteExpression(AbstractExpression node)
        {
            return node.NodeType switch
            {
                AbstractNodeType.ERROR_EXPRESSION => RewriteErrorExpression((AbstractErrorExpression)node),
                AbstractNodeType.COMPOUND_ASSIGNMENT_EXPRESSION => RewriteCompoundAssignmentExpression((AbstractCompoundAssignmentExpression)node),
                AbstractNodeType.UNARY_EXPRESSION => RewriteUnaryExpression((AbstractUnaryExpression)node),
                AbstractNodeType.LITERAL_EXPRESSION => RewriteLiteralExpression((AbstractLiteralExpression)node),
                AbstractNodeType.BINARY_EXPRESSION => RewriteBinaryExpression((AbstractBinaryExpression)node),
                AbstractNodeType.VARIABLE_EXPRESSION => RewriteVariableExpression((AbstractVariableExpression)node),
                AbstractNodeType.ASSIGNMENT_EXPRESSION => RewriteAssignmentExpression((AbstractAssignmentExpression)node),
                AbstractNodeType.CALL_EXPRESSION => RewriteCallExpression((AbstractCallExpression)node),
                AbstractNodeType.CONVERSION_EXPRESSION => RewriteConversionExpression((AbstractConversionExpression)node),
                _ => throw new Exception($"Unexpected node: {node.NodeType}"),
            };
        }

        protected virtual AbstractExpression RewriteConversionExpression(AbstractConversionExpression node)
        {
            AbstractExpression? expression = RewriteExpression(node.Expression);
            if (expression == node.Expression)
                return node;

            return new AbstractConversionExpression(node.Syntax, node.Type, expression);
        }

        protected virtual AbstractStatement RewriteConditionalGotoStatement(AbstractConditionalGotoStatement node)
        {
            AbstractExpression? condition = RewriteExpression(node.Condition);
            if (condition == node.Condition)
                return node;
            return new AbstractConditionalGotoStatement(node.Syntax, node.Label, condition, node.JumpIfTrue);
        }

        protected virtual AbstractStatement RewriteGotoStatement(AbstractGotoStatement node)
        {
            return node;
        }

        protected virtual AbstractStatement RewriteLabelStatement(AbstractLabelStatement node)
        {
            return node;
        }

        protected virtual AbstractStatement RewriteForStatement(AbstractForStatement node)
        {
            AbstractExpression? lowerBound = RewriteExpression(node.LowerBound);
            AbstractExpression? upperBound = RewriteExpression(node.UpperBound);
            AbstractStatement? body = RewriteStatement(node.Body);
            if (lowerBound == node.LowerBound && upperBound == node.UpperBound && body == node.Body)
                return node;

            return new AbstractForStatement(node.Syntax, node.Variable, lowerBound, upperBound, body, node.BreakLabel, node.ContinueLabel);
        }

        protected virtual AbstractStatement RewriteWhileStatement(AbstractWhileStatement node)
        {
            AbstractExpression? condition = RewriteExpression(node.Condition);
            AbstractStatement? body = RewriteStatement(node.Body);
            if (condition == node.Condition && body == node.Body)
                return node;

            return new AbstractWhileStatement(node.Syntax, condition, body, node.BreakLabel, node.ContinueLabel);
        }

        protected virtual AbstractStatement RewriteDoWhileStatement(AbstractDoWhileStatement node)
        {
            AbstractStatement? body = RewriteStatement(node.Body);
            AbstractExpression? condition = RewriteExpression(node.Condition);
            if (condition == node.Condition && body == node.Body)
                return node;

            return new AbstractDoWhileStatement(node.Syntax, body, condition, node.BreakLabel, node.ContinueLabel);
        }

        protected virtual AbstractStatement RewriteVariableDeclarationStatement(AbstractVariableDeclaration node)
        {
            AbstractExpression? initializer = RewriteExpression(node.Initializer);
            if (initializer == node.Initializer)
                return node;

            return new AbstractVariableDeclaration(node.Syntax, node.Variable, initializer);
        }

        protected virtual AbstractStatement RewriteIfStatement(AbstractIfStatement node)
        {
            AbstractExpression? condition = RewriteExpression(node.Condition);
            AbstractStatement? thenStatement = RewriteStatement(node.ThenStatement);
            AbstractStatement? elseStatement = node.ElseStatement == null ? null : RewriteStatement(node.ElseStatement);

            if (condition == node.Condition && thenStatement == node.ThenStatement && elseStatement == node.ElseStatement)
                return node;

            return new AbstractIfStatement(node.Syntax, condition, thenStatement, elseStatement);
        }

        protected virtual AbstractStatement RewriteExpressionStatement(AbstractExpressionStatement node)
        {
            AbstractExpression expression = RewriteExpression(node.Expression);
            if (expression == node.Expression)
                return node;

            return new AbstractExpressionStatement(node.Syntax, expression);
        }

        protected virtual AbstractStatement RewriteBlockStatement(AbstractBlockStatement node)
        {
            ImmutableArray<AbstractStatement>.Builder? builder = null;
            for (int i = 0; i < node.Statements.Length; i++)
            {
                AbstractStatement oldStatement = node.Statements[i];
                AbstractStatement? newStatement = RewriteStatement(oldStatement);
                if (oldStatement != newStatement && builder == null)
                {
                    builder = ImmutableArray.CreateBuilder<AbstractStatement>(node.Statements.Length);
                    for (int j = 0; j < i; j++)
                        builder.Add(node.Statements[j]);
                }

                builder?.Add(newStatement);
            }

            if (builder == null)
                return node;
            return new AbstractBlockStatement(node.Syntax, builder.MoveToImmutable());
        }

        protected virtual AbstractExpression RewriteErrorExpression(AbstractErrorExpression node)
        {
            return node;
        }

        protected virtual AbstractExpression RewriteLiteralExpression(AbstractLiteralExpression node)
        {
            return node;
        }

        protected virtual AbstractExpression RewriteVariableExpression(AbstractVariableExpression node)
        {
            return node;
        }

        protected virtual AbstractExpression RewriteAssignmentExpression(AbstractAssignmentExpression node)
        {
            AbstractExpression expression = RewriteExpression(node.Expression);
            if (expression == node.Expression)
                return node;

            return new AbstractAssignmentExpression(node.Syntax, node.Variable, expression);
        }

        protected virtual AbstractExpression RewriteCompoundAssignmentExpression(AbstractCompoundAssignmentExpression node)
        {
            AbstractExpression expression = RewriteExpression(node.Expression);
            if (expression == node.Expression)
                return node;

            return new AbstractCompoundAssignmentExpression(node.Syntax, node.Variable, node.Operator, expression);
        }

        protected virtual AbstractExpression RewriteUnaryExpression(AbstractUnaryExpression node)
        {
            AbstractExpression operand = RewriteExpression(node.Operand);
            if (operand == node.Operand)
                return node;

            return new AbstractUnaryExpression(node.Syntax, node.Operator, operand);
        }

        protected virtual AbstractExpression RewriteBinaryExpression(AbstractBinaryExpression node)
        {
            AbstractExpression? left = RewriteExpression(node.Left);
            AbstractExpression? right = RewriteExpression(node.Right);

            if (left == node.Left && right == node.Right)
                return node;

            return new AbstractBinaryExpression(node.Syntax, left, node.Operator, right);
        }

        protected virtual AbstractExpression RewriteCallExpression(AbstractCallExpression node)
        {
            ImmutableArray<AbstractExpression>.Builder? builder = null;
            for (int i = 0; i < node.Arguments.Length; i++)
            {
                AbstractExpression? oldArgument = node.Arguments[i];
                AbstractExpression? newArgument = RewriteExpression(oldArgument);

                if (oldArgument != newArgument && builder == null)
                {
                    builder = ImmutableArray.CreateBuilder<AbstractExpression>(node.Arguments.Length);
                    for (int j = 0; j < i; j++)
                        builder.Add(node.Arguments[j]);
                }

                builder?.Add(newArgument);
            }

            if (builder == null)
                return node;

            return new AbstractCallExpression(node.Syntax, node.Function, builder.MoveToImmutable());
        }

        protected virtual AbstractStatement RewriteReturnStatement(AbstractReturnStatement node)
        {
            AbstractExpression? expression = node.Expression == null ? null : RewriteExpression(node.Expression);
            if (expression == node.Expression)
                return node;
            return new AbstractReturnStatement(node.Syntax, expression);
        }
    }
}