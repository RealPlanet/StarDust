using StarDust.Code.AST.Expressions;
using StarDust.Code.AST.Nodes;
using StarDust.Code.AST.Statements;
using StarDust.Code.Symbols;
using StarDust.Code.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;

namespace StarDust.Code.AST.Factory
{
    internal static class AbstractNodeFactory
    {
        public static AbstractBlockStatement Block(Node syntax, params AbstractStatement[] statements)
        {
            return new(syntax, ImmutableArray.Create(statements));
        }

        public static AbstractVariableDeclaration VariableDeclaration(Node syntax, VariableSymbol symbol, AbstractExpression initializer)
        {
            return new(syntax, symbol, initializer);
        }

        public static AbstractVariableDeclaration VariableDeclaration(Node syntax, string name, AbstractExpression initializer)
        {
            return VariableDeclarationInternal(syntax, name, initializer, isReadOnly: false);
        }

        public static AbstractVariableDeclaration ConstantDeclaration(Node syntax, string name, AbstractExpression initializer)
        {
            return VariableDeclarationInternal(syntax, name, initializer, isReadOnly: true);
        }

        private static AbstractVariableDeclaration VariableDeclarationInternal(Node syntax, string name, AbstractExpression initializer, bool isReadOnly)
        {
            LocalVariableSymbol local = new(name, isReadOnly, initializer.Type, initializer.ConstantValue);
            return new AbstractVariableDeclaration(syntax, local, initializer);
        }

        public static AbstractWhileStatement While(Node syntax, AbstractExpression condition, AbstractStatement body, AbstractLabel breakLabel, AbstractLabel continueLabel)
        {
            return new(syntax, condition, body, breakLabel, continueLabel);
        }

        public static AbstractGotoStatement Goto(Node syntax, AbstractLabel label)
        {
            return new(syntax, label);
        }

        public static AbstractConditionalGotoStatement GotoTrue(Node syntax, AbstractLabel label, AbstractExpression condition)
        {
            return new(syntax, label, condition, jumpIfTrue: true);
        }

        public static AbstractConditionalGotoStatement GotoFalse(Node syntax, AbstractLabel label, AbstractExpression condition)
        {
            return new(syntax, label, condition, jumpIfTrue: false);
        }

        public static AbstractLabelStatement Label(Node syntax, AbstractLabel label)
        {
            return new(syntax, label);
        }

        public static AbstractNopStatement Nop(Node syntax)
        {
            return new(syntax);
        }

        public static AbstractAssignmentExpression Assignment(Node syntax, VariableSymbol variable, AbstractExpression expression)
        {
            return new(syntax, variable, expression);
        }

        public static AbstractBinaryExpression Binary(Node syntax, AbstractExpression left, SyntaxType kind, AbstractExpression right)
        {
            AbstractBinaryOperator op = AbstractBinaryOperator.Bind(kind, left.Type, right.Type)!;
            return Binary(syntax, left, op, right);
        }

        public static AbstractBinaryExpression Binary(Node syntax, AbstractExpression left, AbstractBinaryOperator op, AbstractExpression right)
        {
            return new(syntax, left, op, right);
        }

        public static AbstractBinaryExpression Add(Node syntax, AbstractExpression left, AbstractExpression right)
        {
            return Binary(syntax, left, SyntaxType.PLUS_TOKEN, right);
        }

        public static AbstractBinaryExpression LessOrEqual(Node syntax, AbstractExpression left, AbstractExpression right)
        {
            return Binary(syntax, left, SyntaxType.LESS_OR_EQUALS_TOKEN, right);
        }

        public static AbstractExpressionStatement Increment(Node syntax, AbstractVariableExpression variable)
        {
            AbstractBinaryExpression increment = Add(syntax, variable, Literal(syntax, 1));
            AbstractAssignmentExpression incrementAssign = new(syntax, variable.Variable, increment);
            return new AbstractExpressionStatement(syntax, incrementAssign);
        }

        public static AbstractUnaryExpression Not(Node syntax, AbstractExpression condition)
        {
            Debug.Assert(condition.Type == TypeSymbol.Bool);

            AbstractUnaryOperator? op = AbstractUnaryOperator.Bind(SyntaxType.BANG_TOKEN, TypeSymbol.Bool);
            Debug.Assert(op != null);
            return new AbstractUnaryExpression(syntax, op, condition);
        }

        public static AbstractVariableExpression Variable(Node syntax, AbstractVariableDeclaration variable)
        {
            return Variable(syntax, variable.Variable);
        }

        public static AbstractVariableExpression Variable(Node syntax, VariableSymbol variable)
        {
            return new(syntax, variable);
        }

        public static AbstractLiteralExpression Literal(Node syntax, object literal)
        {
            Debug.Assert(literal is string || literal is bool || literal is int);

            return new AbstractLiteralExpression(syntax, literal);
        }
    }
}