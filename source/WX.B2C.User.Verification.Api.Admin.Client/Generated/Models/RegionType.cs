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
    /// Defines values for RegionType.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RegionType
    {
        [EnumMember(Value = "Global")]
        Global,
        [EnumMember(Value = "Region")]
        Region,
        [EnumMember(Value = "Country")]
        Country,
        [EnumMember(Value = "State")]
        State
    }
    internal static class RegionTypeEnumExtension
    {
        internal static string ToSerializedValue(this RegionType? value)
        {
            return value == null ? null : ((RegionType)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this RegionType value)
        {
            switch( value )
            {
                case RegionType.Global:
                    return "Global";
                case RegionType.Region:
                    return "Region";
                case RegionType.Country:
                    return "Country";
                case RegionType.State:
                    return "State";
            }
            return null;
        }

        internal static RegionType? ParseRegionType(this string value)
        {
            switch( value )
            {
                case "Global":
                    return RegionType.Global;
                case "Region":
                    return RegionType.Region;
                case "Country":
                    return RegionType.Country;
                case "State":
                    return RegionType.State;
            }
            return null;
        }
    }
}
