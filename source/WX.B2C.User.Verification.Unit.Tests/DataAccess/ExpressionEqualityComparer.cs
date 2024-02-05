using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WX.B2C.User.Verification.Unit.Tests.DataAccess
{
    internal class ExpressionEqualityComparer : IEqualityComparer<Expression>
    {
        public bool Equals(Expression x, Expression y) => EqualsRecursive(x, y);

        private bool EqualsRecursive(Expression x, Expression y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x.GetType() != y.GetType() ||
                x.Type != y.Type ||
                x.NodeType != y.NodeType)
                return false;

            if (x is LambdaExpression lambdaExpressionX && y is LambdaExpression lambdaExpressionY)
            {
                return AreAllArgumentsEqual(lambdaExpressionX.Parameters, lambdaExpressionY.Parameters) &&
                       Equals(lambdaExpressionX.Body, lambdaExpressionY.Body);
            }

            if (x is BinaryExpression binaryExpressionX && y is BinaryExpression binaryExpressionY)
            {
                return binaryExpressionX.Method == binaryExpressionY.Method &&
                       Equals(binaryExpressionX.Left, binaryExpressionY.Left) &&
                       Equals(binaryExpressionX.Right, binaryExpressionY.Right);
            }

            if (x is UnaryExpression unaryExpressionX && y is UnaryExpression unaryExpressionY)
            {
                return unaryExpressionX.Method == unaryExpressionY.Method &&
                       Equals(unaryExpressionX.Operand, unaryExpressionY.Operand);
            }

            if (x is MethodCallExpression methodCallExpressionX && y is MethodCallExpression methodCallExpressionY)
            {
                return AreAllArgumentsEqual(methodCallExpressionX.Arguments, methodCallExpressionY.Arguments) &&
                       methodCallExpressionX.Method == methodCallExpressionY.Method &&
                       Equals(methodCallExpressionX.Object, methodCallExpressionY.Object);
            }

            if (x is ConditionalExpression conditionalExpressionX && y is ConditionalExpression conditionalExpressionY)
            {
                return Equals(conditionalExpressionX.Test, conditionalExpressionY.Test) &&
                       Equals(conditionalExpressionX.IfTrue, conditionalExpressionY.IfTrue) &&
                       Equals(conditionalExpressionX.IfFalse, conditionalExpressionY.IfFalse);
            }

            if (x is InvocationExpression invocationExpressionX && y is InvocationExpression invocationExpressionY)
            {
                return AreAllArgumentsEqual(invocationExpressionX.Arguments, invocationExpressionY.Arguments) &&
                       Equals(invocationExpressionX.Expression, invocationExpressionY.Expression);
            }

            if (x is MemberExpression memberExpressionX && y is MemberExpression memberExpressionY)
            {
                return memberExpressionX.Member == memberExpressionY.Member &&
                       Equals(memberExpressionX.Expression, memberExpressionY.Expression);
            }

            if (x is ConstantExpression constantExpressionX && y is ConstantExpression constantExpressionY)
            {
                return constantExpressionX.Value.Equals(constantExpressionY.Value);
            }

            if (x is ParameterExpression parameterExpressionX && y is ParameterExpression parameterExpressionY)
            {
                return parameterExpressionX.Name == parameterExpressionY.Name;
            }

            if (x is NewExpression newExpressionX && y is NewExpression newExpressionY)
            {
                return AreAllArgumentsEqual(newExpressionX.Arguments, newExpressionY.Arguments) &&
                       newExpressionX.Constructor == newExpressionY.Constructor;
            }

            return false;
        }

        private bool AreAllArgumentsEqual<T>(IEnumerable<T> xArguments, IEnumerable<T> yArguments)
            where T : Expression
        {
            var argumentEnumeratorX = xArguments.GetEnumerator();
            var argumentEnumeratorY = yArguments.GetEnumerator();

            var haveNotEnumeratedAllOfX = argumentEnumeratorX.MoveNext();
            var haveNotEnumeratedAllOfY = argumentEnumeratorY.MoveNext();

            var areAllArgumentsEqual = true;
            while (haveNotEnumeratedAllOfX && haveNotEnumeratedAllOfY && areAllArgumentsEqual)
            {
                areAllArgumentsEqual = Equals(argumentEnumeratorX.Current, argumentEnumeratorY.Current);
                haveNotEnumeratedAllOfX = argumentEnumeratorX.MoveNext();
                haveNotEnumeratedAllOfY = argumentEnumeratorY.MoveNext();
            }

            if (haveNotEnumeratedAllOfX || haveNotEnumeratedAllOfY)
                return false;

            return areAllArgumentsEqual;
        }

        public int GetHashCode(Expression x)
        {
            if (x is LambdaExpression lambdaExpressionX)
            {
                return XorHashCodes(lambdaExpressionX.Parameters) ^
                       GetHashCode(lambdaExpressionX.Body);
            }

            if (x is BinaryExpression binaryExpressionX)
            {
                var methodHashCode = binaryExpressionX.Method != null
                    ? binaryExpressionX.Method.GetHashCode()
                    : binaryExpressionX.NodeType.GetHashCode();

                return methodHashCode ^
                       GetHashCode(binaryExpressionX.Left) ^
                       GetHashCode(binaryExpressionX.Right);
            }

            if (x is UnaryExpression unaryExpressionX)
            {
                var methodHashCode = unaryExpressionX.Method != null
                    ? unaryExpressionX.Method.GetHashCode()
                    : unaryExpressionX.NodeType.GetHashCode();

                return methodHashCode ^
                       GetHashCode(unaryExpressionX.Operand);
            }

            if (x is MethodCallExpression methodCallExpressionX)
            {
                return XorHashCodes(methodCallExpressionX.Arguments) ^
                       methodCallExpressionX.Method.GetHashCode() ^
                       GetHashCode(methodCallExpressionX.Object);
            }

            if (x is ConditionalExpression conditionalExpressionX)
            {
                return
                    GetHashCode(conditionalExpressionX.Test) ^
                    GetHashCode(conditionalExpressionX.IfTrue) ^
                    GetHashCode(conditionalExpressionX.IfFalse);
            }

            if (x is InvocationExpression invocationExpressionX)
            {
                return
                    XorHashCodes(invocationExpressionX.Arguments) ^
                    GetHashCode(invocationExpressionX.Expression);
            }

            if (x is MemberExpression memberExpressionX)
            {
                return
                    memberExpressionX.Member.GetHashCode() ^
                    GetHashCode(memberExpressionX.Expression);
            }

            if (x is ConstantExpression constantExpressionX)
            {
                return constantExpressionX.Value != null
                    ? constantExpressionX.Value.GetHashCode()
                    : constantExpressionX.GetHashCode();
            }

            if (x is NewExpression newExpressionX)
            {
                return
                    XorHashCodes(newExpressionX.Arguments) ^
                    newExpressionX.Constructor.GetHashCode();
            }

            return 0;
        }

        private int XorHashCodes<T>(IEnumerable<T> expressions) where T : Expression =>
            expressions.Aggregate(0, (current, expression) => current ^ GetHashCode(expression));
    }
}