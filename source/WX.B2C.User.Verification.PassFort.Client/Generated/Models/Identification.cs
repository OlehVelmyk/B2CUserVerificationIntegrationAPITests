// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Identification
    /// </summary>
    /// <remarks>
    /// Identification information for a PEP, such as Passport
    /// </remarks>
    public partial class Identification
    {
        /// <summary>
        /// Initializes a new instance of the Identification class.
        /// </summary>
        public Identification()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Identification class.
        /// </summary>
        /// <param name="type">Type of identification, such as Passport</param>
        /// <param name="values">Identification values</param>
        public Identification(string type = default(string), IList<IdentificationValuesItem> values = default(IList<IdentificationValuesItem>))
        {
            Type = type;
            Values = values;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets type of identification, such as Passport
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets identification values
        /// </summary>
        [JsonProperty(PropertyName = "values")]
        public IList<IdentificationValuesItem> Values { get; set; }

    }
}
