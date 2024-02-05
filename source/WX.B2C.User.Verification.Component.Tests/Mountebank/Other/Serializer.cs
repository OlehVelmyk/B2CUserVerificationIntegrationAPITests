using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Mountebank.Other
{
    internal static class Serializer
    {
        private static JsonSerializerSettings SerializationSettings = new();
        private static JsonSerializerSettings DeserializationSettings = new();

        static Serializer()
        {
            SerializationSettings.Converters.Add(new PolymorphicSerializeJsonConverter<Report>("name"));
            DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<Report>("name"));
        }

        public static string Serialize<T>(T value) => JsonConvert.SerializeObject(value, SerializationSettings);

        public static T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, DeserializationSettings);
    }
}
