// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class DocumentNumber
    {
        /// <summary>
        /// Initializes a new instance of the DocumentNumber class.
        /// </summary>
        public DocumentNumber()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the DocumentNumber class.
        /// </summary>
        public DocumentNumber(string value = default(string), string type = default(string))
        {
            Value = value;
            Type = type;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

    }
}