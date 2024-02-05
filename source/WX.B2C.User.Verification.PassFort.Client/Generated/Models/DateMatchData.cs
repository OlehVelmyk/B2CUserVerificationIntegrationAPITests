// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// DateMatchData
    /// </summary>
    /// <remarks>
    /// Information about the match against the input date
    /// </remarks>
    public partial class DateMatchData
    {
        /// <summary>
        /// Initializes a new instance of the DateMatchData class.
        /// </summary>
        public DateMatchData()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the DateMatchData class.
        /// </summary>
        /// <param name="type">Possible values include: 'DOB', 'DECEASED',
        /// 'END_OF_PEP', 'END_OF_RCA_TO_PEP'</param>
        public DateMatchData(string date, string type = default(string))
        {
            Type = type;
            Date = date;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'DOB', 'DECEASED',
        /// 'END_OF_PEP', 'END_OF_RCA_TO_PEP'
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Date == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Date");
            }
        }
    }
}