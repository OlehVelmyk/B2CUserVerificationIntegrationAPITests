using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WX.B2C.User.Verification.Facade.EventHandlers.Mappers
{
    internal interface IAuditDataSerializer
    {
        public string Serialize<T>(T data);
    }

    internal class AuditDataSerializer : IAuditDataSerializer
    {
        private readonly JsonSerializerSettings _jsonSerializerOptions;

        public AuditDataSerializer()
        {
            _jsonSerializerOptions = new JsonSerializerSettings
            {
                Converters = { new StringEnumConverter() },
            };
        }

        public string Serialize<T>(T data) =>
            JsonConvert.SerializeObject(data, _jsonSerializerOptions);
    }
}