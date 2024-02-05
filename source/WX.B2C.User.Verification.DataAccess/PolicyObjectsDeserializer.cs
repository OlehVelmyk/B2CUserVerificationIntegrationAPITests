using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WX.B2C.User.Verification.DataAccess
{
    internal interface IPolicyObjectsDeserializer
    {
        T Deserialize<T>(string value);
    }

    internal class PolicyObjectsDeserializer
        : IPolicyObjectsDeserializer
    {
        private readonly JsonSerializerSettings _jsonSerializerOptions;

        public PolicyObjectsDeserializer()
        {
            _jsonSerializerOptions = new JsonSerializerSettings
            {
                Converters = { new StringEnumConverter() },
            };
        }

        public T Deserialize<T>(string value) =>
            JsonConvert.DeserializeObject<T>(value, _jsonSerializerOptions);
    }
}
