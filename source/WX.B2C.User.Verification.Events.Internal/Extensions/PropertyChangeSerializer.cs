using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WX.B2C.User.Verification.Events.Internal.Extensions
{
    public class PropertyChangeSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> {new StringEnumConverter()}
        };

        public static string Serialize(object value)
        {
            if (value == null)
                return null;

            return JsonConvert.SerializeObject(value, Settings);
        }

        public static T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonConvert.DeserializeObject<T>(json, Settings);
        }
    }
}