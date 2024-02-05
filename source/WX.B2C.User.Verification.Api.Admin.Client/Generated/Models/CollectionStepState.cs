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
    /// Defines values for CollectionStepState.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CollectionStepState
    {
        [EnumMember(Value = "Requested")]
        Requested,
        [EnumMember(Value = "InReview")]
        InReview,
        [EnumMember(Value = "Completed")]
        Completed,
        [EnumMember(Value = "Cancelled")]
        Cancelled
    }
    internal static class CollectionStepStateEnumExtension
    {
        internal static string ToSerializedValue(this CollectionStepState? value)
        {
            return value == null ? null : ((CollectionStepState)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this CollectionStepState value)
        {
            switch( value )
            {
                case CollectionStepState.Requested:
                    return "Requested";
                case CollectionStepState.InReview:
                    return "InReview";
                case CollectionStepState.Completed:
                    return "Completed";
                case CollectionStepState.Cancelled:
                    return "Cancelled";
            }
            return null;
        }

        internal static CollectionStepState? ParseCollectionStepState(this string value)
        {
            switch( value )
            {
                case "Requested":
                    return CollectionStepState.Requested;
                case "InReview":
                    return CollectionStepState.InReview;
                case "Completed":
                    return CollectionStepState.Completed;
                case "Cancelled":
                    return CollectionStepState.Cancelled;
            }
            return null;
        }
    }
}