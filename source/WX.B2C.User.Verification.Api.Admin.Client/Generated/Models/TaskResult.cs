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
    /// Defines values for TaskResult.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TaskResult
    {
        [EnumMember(Value = "Passed")]
        Passed,
        [EnumMember(Value = "Failed")]
        Failed
    }
    internal static class TaskResultEnumExtension
    {
        internal static string ToSerializedValue(this TaskResult? value)
        {
            return value == null ? null : ((TaskResult)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this TaskResult value)
        {
            switch( value )
            {
                case TaskResult.Passed:
                    return "Passed";
                case TaskResult.Failed:
                    return "Failed";
            }
            return null;
        }

        internal static TaskResult? ParseTaskResult(this string value)
        {
            switch( value )
            {
                case "Passed":
                    return TaskResult.Passed;
                case "Failed":
                    return TaskResult.Failed;
            }
            return null;
        }
    }
}