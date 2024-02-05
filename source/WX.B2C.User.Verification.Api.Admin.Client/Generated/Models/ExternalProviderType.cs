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
    /// Defines values for ExternalProviderType.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExternalProviderType
    {
        [EnumMember(Value = "PassFort")]
        PassFort,
        [EnumMember(Value = "Onfido")]
        Onfido,
        [EnumMember(Value = "LexisNexis")]
        LexisNexis
    }
    internal static class ExternalProviderTypeEnumExtension
    {
        internal static string ToSerializedValue(this ExternalProviderType? value)
        {
            return value == null ? null : ((ExternalProviderType)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this ExternalProviderType value)
        {
            switch( value )
            {
                case ExternalProviderType.PassFort:
                    return "PassFort";
                case ExternalProviderType.Onfido:
                    return "Onfido";
                case ExternalProviderType.LexisNexis:
                    return "LexisNexis";
            }
            return null;
        }

        internal static ExternalProviderType? ParseExternalProviderType(this string value)
        {
            switch( value )
            {
                case "PassFort":
                    return ExternalProviderType.PassFort;
                case "Onfido":
                    return ExternalProviderType.Onfido;
                case "LexisNexis":
                    return ExternalProviderType.LexisNexis;
            }
            return null;
        }
    }
}
