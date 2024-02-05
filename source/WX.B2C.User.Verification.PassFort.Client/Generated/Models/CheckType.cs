// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines values for CheckType.
    /// </summary>
    /// <summary>
    /// Determine base value for a given allowed value if exists, else return
    /// the value itself
    /// </summary>
    [JsonConverter(typeof(CheckTypeConverter))]
    public struct CheckType : System.IEquatable<CheckType>
    {
        private CheckType(string underlyingValue)
        {
            UnderlyingValue=underlyingValue;
        }

        public static readonly CheckType IDENTITYCHECK = "IDENTITY_CHECK";

        public static readonly CheckType DOCUMENTVERIFICATION = "DOCUMENT_VERIFICATION";

        public static readonly CheckType DOCUMENTFETCH = "DOCUMENT_FETCH";

        public static readonly CheckType PEPSANDSANCTIONSSCREEN = "PEPS_AND_SANCTIONS_SCREEN";

        public static readonly CheckType VISACHECK = "VISA_CHECK";

        public static readonly CheckType CRABANKACCOUNTVERIFICATION = "CRA_BANK_ACCOUNT_VERIFICATION";

        public static readonly CheckType DEVICEFRAUDDETECTION = "DEVICE_FRAUD_DETECTION";

        public static readonly CheckType FRAUDSCREENING = "FRAUD_SCREENING";

        public static readonly CheckType GEOCODING = "GEOCODING";

        public static readonly CheckType COMPANYREGISTRY = "COMPANY_REGISTRY";

        public static readonly CheckType COMPANYCHARITIESREGISTRY = "COMPANY_CHARITIES_REGISTRY";

        public static readonly CheckType COMPANYOWNERSHIP = "COMPANY_OWNERSHIP";

        public static readonly CheckType COMPANYDATA = "COMPANY_DATA";

        public static readonly CheckType COMPANYFILINGS = "COMPANY_FILINGS";

        public static readonly CheckType COMPANYFILINGPURCHASE = "COMPANY_FILING_PURCHASE";

        public static readonly CheckType COMPANYPEPSANDSANCTIONSSCREEN = "COMPANY_PEPS_AND_SANCTIONS_SCREEN";

        public static readonly CheckType INDIVIDUALMANUALFORM = "INDIVIDUAL_MANUAL_FORM";

        public static readonly CheckType COMPANYMANUALFORM = "COMPANY_MANUAL_FORM";

        public static readonly CheckType COMPANYDOCUMENTVERIFICATION = "COMPANY_DOCUMENT_VERIFICATION";

        public static readonly CheckType COMPANYFRAUDSCREENING = "COMPANY_FRAUD_SCREENING";

        public static readonly CheckType COMPANYMERCHANTFRAUDSCREENING = "COMPANY_MERCHANT_FRAUD_SCREENING";


        /// <summary>
        /// Underlying value of enum CheckType
        /// </summary>
        private readonly string UnderlyingValue;

        /// <summary>
        /// Returns string representation for CheckType
        /// </summary>
        public override string ToString()
        {
            return UnderlyingValue == null ? null : UnderlyingValue.ToString();
        }

        /// <summary>
        /// Compares enums of type CheckType
        /// </summary>
        public bool Equals(CheckType e)
        {
            return UnderlyingValue.Equals(e.UnderlyingValue);
        }

        /// <summary>
        /// Implicit operator to convert string to CheckType
        /// </summary>
        public static implicit operator CheckType(string value)
        {
            return new CheckType(value);
        }

        /// <summary>
        /// Implicit operator to convert CheckType to string
        /// </summary>
        public static implicit operator string(CheckType e)
        {
            return e.UnderlyingValue;
        }

        /// <summary>
        /// Overriding == operator for enum CheckType
        /// </summary>
        public static bool operator == (CheckType e1, CheckType e2)
        {
            return e2.Equals(e1);
        }

        /// <summary>
        /// Overriding != operator for enum CheckType
        /// </summary>
        public static bool operator != (CheckType e1, CheckType e2)
        {
            return !e2.Equals(e1);
        }

        /// <summary>
        /// Overrides Equals operator for CheckType
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is CheckType && Equals((CheckType)obj);
        }

        /// <summary>
        /// Returns for hashCode CheckType
        /// </summary>
        public override int GetHashCode()
        {
            return UnderlyingValue.GetHashCode();
        }

    }
}
