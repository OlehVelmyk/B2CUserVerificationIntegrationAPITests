using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Component.Tests.Models;
using WX.B2C.User.Verification.Component.Tests.Models.Enums;

namespace WX.B2C.User.Verification.Component.Tests.Extensions
{
    internal static class FsCheckExtensions
    {
        public static T Sample<T>(this Gen<T> gen, Rnd? fsSeed = null) => gen.Sample(1, fsSeed)[0];

        public static Gen<T> Override<T, TProperty>(this Gen<T> gen,
                                                    Gen<TProperty> newValueGen, 
                                                    Expression<Func<T, TProperty>> propertySelector,
                                                    Func<T, bool> predicate = null) where T : class
        {
            if (newValueGen is null) 
                throw new ArgumentNullException(nameof(newValueGen)); 

            return from newValue in newValueGen
                   from model in gen.Override(newValue, propertySelector, predicate)
                   select model;
        }        

        public static Gen<T> Override<T, TProperty>(this Gen<T> gen,
                                                    TProperty newValue, 
                                                    Expression<Func<T, TProperty>> propertySelector,
                                                    Func<T, bool> predicate = null) where T : class
        {
            if (gen is null) 
                throw new ArgumentNullException(nameof(gen));
            if (propertySelector is null) 
                throw new ArgumentNullException(nameof(propertySelector));

            return from model in gen
                   let _ = Executor(model, newValue, propertySelector, predicate)
                   select model;
        }

        private static bool Executor<T, TProperty>(T rootModel,
                                                   TProperty newValue, 
                                                   Expression<Func<T, TProperty>> propertySelector,
                                                   Func<T, bool> predicate = null) where T : class
        {
            if (propertySelector.Body is not MemberExpression memberExpression || memberExpression.Member is not PropertyInfo propertyInfo)
                throw new InvalidOperationException("Expression member should be property.");
            if (!propertyInfo.CanWrite)
                throw new InvalidOperationException("Property should be writable.");

            if (rootModel is null) return false;            
            if(!predicate?.Invoke(rootModel) ?? false) return false;

            var model = GetModel(rootModel, memberExpression);
            propertyInfo.SetValue(model, newValue, null);
            return true;
        }

        /// <summary>
        /// Find model for which need to set new value
        /// </summary>
        /// <example>
        /// ui => ui.Address.Country
        /// in such lambda we set new value for 'Country' property in 'Address', originally 'Address' will be changed, not 'ui'
        /// so method returns 'Address' object
        /// </example>
        private static object GetModel(object rootModel, MemberExpression memberExpression)
        {
            var stack = new Stack<PropertyInfo>();
            while (memberExpression.Expression is MemberExpression me)
            {
                var propertyInfo = me.Member as PropertyInfo ?? throw new InvalidOperationException(BuildNotPropertyMessage(me.Member));
                if (!propertyInfo.CanRead) 
                    throw new InvalidOperationException(BuildNotReadableMessage(propertyInfo));

                stack.Push(propertyInfo);
                memberExpression = me;
            }

            var model = rootModel;
            while(stack.TryPop(out var propertyInfo))
                model = propertyInfo.GetValue(model);

            return model;

            string BuildNotPropertyMessage(MemberInfo memberInfo) =>
                $"{memberInfo.ReflectedType?.Name}.{memberInfo.Name} is not property.";

            string BuildNotReadableMessage(PropertyInfo propertyInfo) =>
                $"{propertyInfo.ReflectedType?.Name}.{propertyInfo.Name} should be readable.";
        }
    }
}
