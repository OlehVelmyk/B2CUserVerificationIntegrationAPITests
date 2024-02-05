using System;
using System.Linq.Expressions;
using System.Reflection;
using Optional;
using Optional.Unsafe;
using WX.B2C.User.Verification.Domain.Profile;

namespace WX.B2C.User.Verification.Core.Services.Extensions
{
    public abstract class PatchResult
    {
        public bool IsPatched { get; private set; }

        public static PatchResult<T> Patched<T>(T oldValue, T newValue) =>
            new() { IsPatched = true, OldValue = oldValue, NewValue = newValue };

        public static PatchResult<T> NotPatched<T>(T oldValue) =>
            new() { IsPatched = false, OldValue = oldValue, NewValue = oldValue };

        public abstract PropertyChange GetChange(string property);
    }

    public class PatchResult<TValue> : PatchResult
    {
        public TValue OldValue { get; set; }

        public TValue NewValue { get; set; }

        public override PropertyChange GetChange(string property) =>
            PropertyChange.Create(property, NewValue, OldValue);
    }

    public static class PatchExtensions
    {
        public static PatchResult<TProperty> Patch<TModel, TProperty>(
            this TModel model,
            Expression<Func<TModel, TProperty>> propertySelector,
            Func<Option<TProperty>> newValueProvider,
            Func<TProperty, bool> filter = null,
            Func<TProperty, TProperty, bool> equalityComparer = null)
            where TModel : class
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (propertySelector == null)
                throw new ArgumentNullException(nameof(propertySelector));
            if (newValueProvider == null)
                throw new ArgumentNullException(nameof(newValueProvider));

            if (!TryGetPropertyInfo(propertySelector.Body, out var propertyInfo))
                throw new InvalidOperationException("Expression member should be property.");
            if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                throw new InvalidOperationException("Property should be readable and writable.");

            equalityComparer ??= Equals;

            var newValueOption = newValueProvider();
            if (filter != null)
                newValueOption = newValueOption.Filter(value => !filter(value));

            var oldValue = (TProperty)propertyInfo.GetValue(model, null);
            if (!newValueOption.HasValue) 
                return PatchResult.NotPatched(oldValue);

            var newValue = newValueOption.ValueOrDefault();
            var isChanged = !equalityComparer(oldValue, newValue);
            if (!isChanged)
                return PatchResult.NotPatched(oldValue);

            propertyInfo.SetValue(model, newValue, null);
            return PatchResult.Patched(oldValue, newValue);
        }

        private static bool TryGetPropertyInfo(Expression expression, out PropertyInfo propertyInfo)
        {
            propertyInfo = default;

            if (expression is MemberExpression memberExpression)
            {
                propertyInfo = memberExpression.Member as PropertyInfo;
                return propertyInfo != null;
            }

            return false;
        }

        private static bool Equals<T>(T oldValue, T newValue) => object.Equals(newValue, oldValue);
    }
}
