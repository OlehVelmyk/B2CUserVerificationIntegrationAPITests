using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Converters
{
    public abstract class PolymorphicDeserializer<TBase> : JsonConverter<TBase>
    {
        private readonly string _discriminatorName;

        protected PolymorphicDeserializer(string discriminatorName)
        {
            if (string.IsNullOrWhiteSpace(discriminatorName))
                throw new ArgumentNullException(nameof(discriminatorName));

            _discriminatorName = discriminatorName.ToLower();
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert == typeof(object))
                return false;

            var isAssignableFrom = typeToConvert.IsAssignableFrom(typeof(TBase));
            return isAssignableFrom;
        }

        public override void Write(Utf8JsonWriter writer, TBase value, JsonSerializerOptions options) => throw new NotSupportedException();

        public override TBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            if (!JsonDocument.TryParseValue(ref reader, out var jsonDocument))
                throw new JsonException("Invalid json document.");

            if (!jsonDocument.RootElement.TryGetProperty(_discriminatorName, out var discriminator))
                throw new JsonException(
                    $"Missing discriminator \"{_discriminatorName}\" for type \"{typeof(TBase).FullName}\".");

            if (!TryGetType(discriminator.GetString(), out var resolvedType))
                throw new JsonException(
                    $"Invalid discriminator value, \"{_discriminatorName}\": \"{discriminator}\".");

            if (!typeof(TBase).IsAssignableFrom(resolvedType))
                throw new InvalidCastException(
                    $"Type {resolvedType} is not derived from {nameof(TBase)}.");

            var json = jsonDocument.RootElement.GetRawText();
            return (TBase)JsonSerializer.Deserialize(json, resolvedType, options);
        }

        protected abstract bool TryGetType(string discriminator, out Type resolvedType);
    }
}