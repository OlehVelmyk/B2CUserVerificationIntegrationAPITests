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
    /// Defines values for ActionType.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActionType
    {
        [EnumMember(Value = "Survey")]
        Survey,
        [EnumMember(Value = "Selfie")]
        Selfie,
        [EnumMember(Value = "TaxResidence")]
        TaxResidence,
        [EnumMember(Value = "Tin")]
        Tin,
        [EnumMember(Value = "ProofOfIdentity")]
        ProofOfIdentity,
        [EnumMember(Value = "ProofOfAddress")]
        ProofOfAddress,
        [EnumMember(Value = "ProofOfFunds")]
        ProofOfFunds,
        [EnumMember(Value = "W9Form")]
        W9Form
    }
    internal static class ActionTypeEnumExtension
    {
        internal static string ToSerializedValue(this ActionType? value)
        {
            return value == null ? null : ((ActionType)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this ActionType value)
        {
            switch( value )
            {
                case ActionType.Survey:
                    return "Survey";
                case ActionType.Selfie:
                    return "Selfie";
                case ActionType.TaxResidence:
                    return "TaxResidence";
                case ActionType.Tin:
                    return "Tin";
                case ActionType.ProofOfIdentity:
                    return "ProofOfIdentity";
                case ActionType.ProofOfAddress:
                    return "ProofOfAddress";
                case ActionType.ProofOfFunds:
                    return "ProofOfFunds";
                case ActionType.W9Form:
                    return "W9Form";
            }
            return null;
        }

        internal static ActionType? ParseActionType(this string value)
        {
            switch( value )
            {
                case "Survey":
                    return ActionType.Survey;
                case "Selfie":
                    return ActionType.Selfie;
                case "TaxResidence":
                    return ActionType.TaxResidence;
                case "Tin":
                    return ActionType.Tin;
                case "ProofOfIdentity":
                    return ActionType.ProofOfIdentity;
                case "ProofOfAddress":
                    return ActionType.ProofOfAddress;
                case "ProofOfFunds":
                    return ActionType.ProofOfFunds;
                case "W9Form":
                    return ActionType.W9Form;
            }
            return null;
        }
    }
}
