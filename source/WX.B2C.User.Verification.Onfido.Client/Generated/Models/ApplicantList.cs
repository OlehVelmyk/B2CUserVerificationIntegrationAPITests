// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class ApplicantList
    {
        /// <summary>
        /// Initializes a new instance of the ApplicantList class.
        /// </summary>
        public ApplicantList()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ApplicantList class.
        /// </summary>
        public ApplicantList(IList<Applicant> applicants = default(IList<Applicant>))
        {
            Applicants = applicants;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "applicants")]
        public IList<Applicant> Applicants { get; set; }

    }
}
