// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;

    using System.Reflection;

    /// <summary>
    /// Defines values for ReportSubResult.
    /// </summary>
    public sealed class ReportSubResultConverter : JsonConverter
    {

        /// <summary>
        /// Returns if objectType can be converted to ReportSubResult by the
        /// converter.
        /// </summary>
        public override bool CanConvert(System.Type objectType)
        {
            return typeof(ReportSubResult).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        /// <summary>
        /// Overrides ReadJson and converts token to ReportSubResult.
        /// </summary>
        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == Newtonsoft.Json.JsonToken.Null)
            {
                return null;
            }
            return (ReportSubResult)serializer.Deserialize<string>(reader);
        }

        /// <summary>
        /// Overriding WriteJson for ReportSubResult for serialization.
        /// </summary>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

    }
}
