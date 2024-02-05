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
    /// Defines values for ApplicationState.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApplicationState
    {
        [EnumMember(Value = "Applied")]
        Applied,
        [EnumMember(Value = "Approved")]
        Approved,
        [EnumMember(Value = "InReview")]
        InReview,
        [EnumMember(Value = "Rejected")]
        Rejected,
        [EnumMember(Value = "Cancelled")]
        Cancelled
    }
    internal static class ApplicationStateEnumExtension
    {
        internal static string ToSerializedValue(this ApplicationState? value)
        {
            return value == null ? null : ((ApplicationState)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this ApplicationState value)
        {
            switch( value )
            {
                case ApplicationState.Applied:
                    return "Applied";
                case ApplicationState.Approved:
                    return "Approved";
                case ApplicationState.InReview:
                    return "InReview";
                case ApplicationState.Rejected:
                    return "Rejected";
                case ApplicationState.Cancelled:
                    return "Cancelled";
            }
            return null;
        }

        internal static ApplicationState? ParseApplicationState(this string value)
        {
            switch( value )
            {
                case "Applied":
                    return ApplicationState.Applied;
                case "Approved":
                    return ApplicationState.Approved;
                case "InReview":
                    return ApplicationState.InReview;
                case "Rejected":
                    return ApplicationState.Rejected;
                case "Cancelled":
                    return ApplicationState.Cancelled;
            }
            return null;
        }
    }
}
