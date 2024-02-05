// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.PassFort.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// ProductApplicationStatusChange
    /// </summary>
    /// <remarks>
    /// Represent a change in a product application status
    /// </remarks>
    public partial class ProductApplicationStatusChange
    {
        /// <summary>
        /// Initializes a new instance of the ProductApplicationStatusChange
        /// class.
        /// </summary>
        public ProductApplicationStatusChange()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ProductApplicationStatusChange
        /// class.
        /// </summary>
        /// <param name="status">Possible values include: 'APPLIED',
        /// 'APPROVED', 'IN_REVIEW', 'REJECTED', 'CANCELLED'</param>
        /// <param name="comment">A comment about the change</param>
        public ProductApplicationStatusChange(string status = default(string), string date = default(string), User decisioner = default(User), string comment = default(string))
        {
            Status = status;
            Date = date;
            Decisioner = decisioner;
            Comment = comment;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'APPLIED', 'APPROVED',
        /// 'IN_REVIEW', 'REJECTED', 'CANCELLED'
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "decisioner")]
        public User Decisioner { get; set; }

        /// <summary>
        /// Gets or sets a comment about the change
        /// </summary>
        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }

    }
}
