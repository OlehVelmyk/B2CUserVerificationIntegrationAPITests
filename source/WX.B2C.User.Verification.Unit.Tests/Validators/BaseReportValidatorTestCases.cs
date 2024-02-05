using System.Collections.Generic;
using System.IO;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WX.B2C.User.Verification.Onfido.Client.Models;

namespace WX.B2C.User.Verification.Unit.Tests.Validators
{
    internal abstract class BaseReportValidatorTestCases
    {
        private static readonly JsonSerializerSettings DeserializationSettings = new()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            ContractResolver = new ReadOnlyJsonContractResolver(),
            Converters = new List<JsonConverter>
            {
                new Iso8601TimeSpanConverter(),
                new PolymorphicDeserializeJsonConverter<Report>("name")
            }
        };

        private static readonly JsonSerializer Deserializer = JsonSerializer.Create(DeserializationSettings);

        public static Report ToReport(JToken json) => ToReport<Report>(json);

        public static TReport ToReport<TReport>(JToken json) where TReport : Report =>
            json?.ToObject<TReport>(Deserializer);

        public static JArray ReadReportsFromJson(string fileName, string testCase)
        {
            var rootPath = Directory.GetCurrentDirectory();
            var path = Path.Combine(rootPath, @"Validators\Reports", fileName);
            using var reader = new StreamReader(path);
            var reports = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
            return reports?[testCase] as JArray;
        }
    }
}