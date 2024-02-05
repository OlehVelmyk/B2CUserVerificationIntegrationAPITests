using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsPocoClass(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsClass
                && type != typeof(string)
                && !typeof(Delegate).IsAssignableFrom(type)
                && !typeof(IEnumerable).IsAssignableFrom(type)
                && !type.IsTuple();
        }

        public static bool IsCollection(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return typeof(IEnumerable<object>).IsAssignableFrom(type);
        }

        public static bool IsTuple(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return typeof(ITuple).IsAssignableFrom(type);
        }

        public static bool IsKeyValuePair(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
        }

        public static Type GetTypeOfCollection(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!typeof(IEnumerable<object>).IsAssignableFrom(type))
                throw new ArgumentException($"Type { type.Name } is not collection.");

            return type.IsGenericType 
                ? type.GenericTypeArguments.First() 
                : type.GetInterface(typeof(IEnumerable<>).Name).GenericTypeArguments.First();
        }

        public static Type FindGenericArgument(this Type type, int index)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!type.IsGenericType)
                throw new ArgumentException($"Type {type} is not generic.");

            return type.GenericTypeArguments.ElementAtOrDefault(index);
        }

        public static bool IsTask(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return typeof(Task).IsAssignableFrom(type);
        }

        public static bool HasChildren(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetCustomAttributes<KnownTypeAttribute>().Any();
        }

        public static IEnumerable<Type> FindChildren(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetCustomAttributes<KnownTypeAttribute>()
                       .Select(attribute => attribute.Type);
        }
    }
}
