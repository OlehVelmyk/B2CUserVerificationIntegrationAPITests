using System;
using System.Linq.Expressions;

namespace WX.B2C.User.Verification.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            var replaceVisitor = new ParameterReplaceVisitor(left.Parameters[0], right.Parameters[0]);
            var wrapped = replaceVisitor.Visit(left.Body);
            var andAlso = Expression.AndAlso(wrapped, right.Body);
            return Expression.Lambda<Func<T, bool>>(andAlso, right.Parameters);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            var replaceVisitor = new ParameterReplaceVisitor(left.Parameters[0], right.Parameters[0]);
            var wrapped = replaceVisitor.Visit(left.Body);
            var andAlso = Expression.OrElse(wrapped, right.Body);
            return Expression.Lambda<Func<T, bool>>(andAlso, right.Parameters);
        }

        private class ParameterReplaceVisitor : ExpressionVisitor
        {
            private readonly Expression _from, _to;

            public ParameterReplaceVisitor(Expression from, Expression to)
            {
                _from = from;
                _to = to;
            }

            public override Expression Visit(Expression node)
            {
                return node == _from ? _to : base.Visit(node);
            }
        }
    }
}
