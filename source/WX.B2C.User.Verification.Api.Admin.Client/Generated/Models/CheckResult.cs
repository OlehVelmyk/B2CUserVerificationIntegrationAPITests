// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Admin.Client.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for CheckResult.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CheckResult
    {
        [EnumMember(Value = "Passed")]
        Passed,
        [EnumMember(Value = "Failed")]
        Failed
    }
    internal static class CheckResultEnumExtension
    {
        internal static string ToSerializedValue(this CheckResult? value)
        {
            return value == null ? null : ((CheckResult)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this CheckResult value)
        {
            switch( value )
            {
                case CheckResult.Passed:
                    return "Passed";
                case CheckResult.Failed:
                    return "Failed";
            }
            return null;
        }

        internal static CheckResult? ParseCheckResult(this string value)
        {
            switch( value )
            {
                case "Passed":
                    return CheckResult.Passed;
                case "Failed":
                    return CheckResult.Failed;
            }
            return null;
        }
    }
}
