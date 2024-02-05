// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines values for DocumentFileType.
    /// </summary>
    /// <summary>
    /// Determine base value for a given allowed value if exists, else return
    /// the value itself
    /// </summary>
    [JsonConverter(typeof(DocumentFileTypeConverter))]
    public struct DocumentFileType : System.IEquatable<DocumentFileType>
    {
        private DocumentFileType(string underlyingValue)
        {
            UnderlyingValue=underlyingValue;
        }

        public static readonly DocumentFileType LIVEVIDEO = "LIVE_VIDEO";

        public static readonly DocumentFileType VIDEOFRAME = "VIDEO_FRAME";


        /// <summary>
        /// Underlying value of enum DocumentFileType
        /// </summary>
        private readonly string UnderlyingValue;

        /// <summary>
        /// Returns string representation for DocumentFileType
        /// </summary>
        public override string ToString()
        {
            return UnderlyingValue == null ? null : UnderlyingValue.ToString();
        }

        /// <summary>
        /// Compares enums of type DocumentFileType
        /// </summary>
        public bool Equals(DocumentFileType e)
        {
            return UnderlyingValue.Equals(e.UnderlyingValue);
        }

        /// <summary>
        /// Implicit operator to convert string to DocumentFileType
        /// </summary>
        public static implicit operator DocumentFileType(string value)
        {
            return new DocumentFileType(value);
        }

        /// <summary>
        /// Implicit operator to convert DocumentFileType to string
        /// </summary>
        public static implicit operator string(DocumentFileType e)
        {
            return e.UnderlyingValue;
        }

        /// <summary>
        /// Overriding == operator for enum DocumentFileType
        /// </summary>
        public static bool operator == (DocumentFileType e1, DocumentFileType e2)
        {
            return e2.Equals(e1);
        }

        /// <summary>
        /// Overriding != operator for enum DocumentFileType
        /// </summary>
        public static bool operator != (DocumentFileType e1, DocumentFileType e2)
        {
            return !e2.Equals(e1);
        }

        /// <summary>
        /// Overrides Equals operator for DocumentFileType
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is DocumentFileType && Equals((DocumentFileType)obj);
        }

        /// <summary>
        /// Returns for hashCode DocumentFileType
        /// </summary>
        public override int GetHashCode()
        {
            return UnderlyingValue.GetHashCode();
        }

    }
}