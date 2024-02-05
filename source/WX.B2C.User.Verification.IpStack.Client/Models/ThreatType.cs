// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.IpStack.Client.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines values for ThreatType.
    /// </summary>
    /// <summary>
    /// Determine base value for a given allowed value if exists, else return
    /// the value itself
    /// </summary>
    [JsonConverter(typeof(ThreatTypeConverter))]
    public struct ThreatType : System.IEquatable<ThreatType>
    {
        private ThreatType(string underlyingValue)
        {
            UnderlyingValue=underlyingValue;
        }

        /// <summary>
        /// Tor System
        /// </summary>
        public static readonly ThreatType Tor = "tor";

        /// <summary>
        /// Fake Crawler
        /// </summary>
        public static readonly ThreatType FakeCrawler = "fake_crawler";

        /// <summary>
        /// Web Scraper
        /// </summary>
        public static readonly ThreatType WebScraper = "web_scraper";

        /// <summary>
        /// Attack Source identified: HTTP
        /// </summary>
        public static readonly ThreatType AttackSource = "attack_source";

        /// <summary>
        /// Attack Source identified: HTTP
        /// </summary>
        public static readonly ThreatType AttackSourceHttp = "attack_source_http";

        /// <summary>
        /// Attack Source identified: Mail
        /// </summary>
        public static readonly ThreatType AttackSourceMail = "attack_source_mail";

        /// <summary>
        /// Attack Source identified: SSH
        /// </summary>
        public static readonly ThreatType AttackSourceSsh = "attack_source_ssh";


        /// <summary>
        /// Underlying value of enum ThreatType
        /// </summary>
        private readonly string UnderlyingValue;

        /// <summary>
        /// Returns string representation for ThreatType
        /// </summary>
        public override string ToString()
        {
            return UnderlyingValue == null ? null : UnderlyingValue.ToString();
        }

        /// <summary>
        /// Compares enums of type ThreatType
        /// </summary>
        public bool Equals(ThreatType e)
        {
            return UnderlyingValue.Equals(e.UnderlyingValue);
        }

        /// <summary>
        /// Implicit operator to convert string to ThreatType
        /// </summary>
        public static implicit operator ThreatType(string value)
        {
            return new ThreatType(value);
        }

        /// <summary>
        /// Implicit operator to convert ThreatType to string
        /// </summary>
        public static implicit operator string(ThreatType e)
        {
            return e.UnderlyingValue;
        }

        /// <summary>
        /// Overriding == operator for enum ThreatType
        /// </summary>
        public static bool operator == (ThreatType e1, ThreatType e2)
        {
            return e2.Equals(e1);
        }

        /// <summary>
        /// Overriding != operator for enum ThreatType
        /// </summary>
        public static bool operator != (ThreatType e1, ThreatType e2)
        {
            return !e2.Equals(e1);
        }

        /// <summary>
        /// Overrides Equals operator for ThreatType
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is ThreatType && Equals((ThreatType)obj);
        }

        /// <summary>
        /// Returns for hashCode ThreatType
        /// </summary>
        public override int GetHashCode()
        {
            return UnderlyingValue.GetHashCode();
        }

    }
}
