using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace WX.B2C.User.Verification.DataAccess.EF.Extensions
{
    internal static class PropertyBuilderExtensions
    {
        private static readonly JsonSerializerSettings TypeNamedSerializerSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            FloatParseHandling = FloatParseHandling.Decimal
        };

        public static PropertyBuilder<TEnum?> HasEnumToStringConversion<TEnum>(this PropertyBuilder<TEnum?> propertyBuilder)
            where TEnum : struct, Enum
        {
            if (propertyBuilder == null)
                throw new ArgumentNullException(nameof(propertyBuilder));

            return propertyBuilder.HasConversion(
                property => property == null ? null : property.ToString(),
                value => value == null ? null : Enum.Parse<TEnum>(value));
        }

        public static PropertyBuilder<TEnum> HasEnumToStringConversion<TEnum>(this PropertyBuilder<TEnum> propertyBuilder)
            where TEnum : struct, Enum
        {
            if (propertyBuilder == null)
                throw new ArgumentNullException(nameof(propertyBuilder));

            return propertyBuilder.HasConversion(
                property => property.ToString(),
                value => Enum.Parse<TEnum>(value));
        }

        public static PropertyBuilder<T> HasTypeNamedJsonSerializerConversion<T>(this PropertyBuilder<T> propertyBuilder)
        {
            if (propertyBuilder == null)
                throw new ArgumentNullException(nameof(propertyBuilder));

            return propertyBuilder.HasConversion(
            data => SerializeObject(data, TypeNamedSerializerSettings),
            json => DeserializeObject<T>(json, TypeNamedSerializerSettings));
        }

        public static PropertyBuilder<T> HasJsonSerializerConversion<T>(this PropertyBuilder<T> propertyBuilder, JsonSerializerSettings settings = null)
        {
            if (propertyBuilder == null)
                throw new ArgumentNullException(nameof(propertyBuilder));

            return propertyBuilder.HasConversion(
                data => SerializeObject(data, settings),
                json => DeserializeObject<T>(json, settings));
        }

        private static string SerializeObject<T>(T data, JsonSerializerSettings settings = null)
        {
            if (data == null)
                return null;

            settings ??= new JsonSerializerSettings();

            return JsonConvert.SerializeObject(data, settings);
        }

        private static T DeserializeObject<T>(string json, JsonSerializerSettings settings = null)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            settings ??= new JsonSerializerSettings();

            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}