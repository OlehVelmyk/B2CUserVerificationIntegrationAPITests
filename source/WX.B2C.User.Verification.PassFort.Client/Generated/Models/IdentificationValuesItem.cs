// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class IdentificationValuesItem
    {
        /// <summary>
        /// Initializes a new instance of the IdentificationValuesItem class.
        /// </summary>
        public IdentificationValuesItem()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the IdentificationValuesItem class.
        /// </summary>
        /// <param name="notes">Notes on identification value</param>
        /// <param name="value">Identification value</param>
        public IdentificationValuesItem(string notes = default(string), string value = default(string))
        {
            Notes = notes;
            Value = value;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets notes on identification value
        /// </summary>
        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets identification value
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

    }
}
