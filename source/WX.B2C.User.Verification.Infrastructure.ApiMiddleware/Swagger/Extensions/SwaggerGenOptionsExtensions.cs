using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Extensions
{
    public static class SwaggerGenOptionsExtensions
    {
        public static void UsePolymorphismFor<T>(this SwaggerGenOptions options)
        {
            options.UsePolymorphismFor<T, string>(
                "$type",
                type => $"{type.FullName}, {type.Assembly.GetName().Name}");
        }

        public static void UsePolymorphismFor<T>(this SwaggerGenOptions options, string discriminatorName)
        {
            options.UsePolymorphismFor<T, string>(
                discriminatorName.ToLower(),
                type => GetDiscriminatorValue(type, discriminatorName));
        }

        public static void UsePolymorphismFor<T, TDiscriminator>(this SwaggerGenOptions options,
                                                                 string discriminatorName,
                                                                 Func<Type, TDiscriminator> discriminatorProvider)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (discriminatorName == null)
                throw new ArgumentNullException(nameof(discriminatorName));
            if (discriminatorProvider == null)
                throw new ArgumentNullException(nameof(discriminatorProvider));

            var baseType = typeof(T);
            var derivedTypes = GetDerivedTypes(baseType);
            options.DocumentFilter<PolymorphicDocumentFilter>(baseType, derivedTypes, discriminatorName);
            options.SchemaFilter<PolymorphicSchemaFilter>(baseType, derivedTypes, discriminatorProvider);
        }

        private static HashSet<Type> GetDerivedTypes(Type baseType)
        {
            var typeAssembly = baseType.GetTypeInfo().Assembly;
            return typeAssembly
                   .GetTypes()
                   .Where(IsDerivedType)
                   .ToHashSet();

            bool IsDerivedType(Type type) => baseType != type && !type.IsAbstract && baseType.IsAssignableFrom(type);
        }

        private static string GetDiscriminatorValue(Type type, string propertyName)
        {
            if (type.GetConstructor(new Type[] { }) == null)
                throw new InvalidOperationException($"{type.Name} doesn't have a default constructor.");

            var instance = Activator.CreateInstance(type);
            var discriminatorProperty = type.GetProperty(propertyName);
            if (discriminatorProperty == null)
                throw new InvalidOperationException($"{type.Name} doesn't have a property with name {propertyName}");

            var discriminatorValue = discriminatorProperty.GetValue(instance, new object[] { });
            return discriminatorValue?.ToString();
        }
    }
}
