// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Admin.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class HostLogLevelDto
    {
        /// <summary>
        /// Initializes a new instance of the HostLogLevelDto class.
        /// </summary>
        public HostLogLevelDto()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the HostLogLevelDto class.
        /// </summary>
        /// <param name="level">Possible values include: 'Verbose', 'Debug',
        /// 'Information', 'Warning', 'Error', 'Fatal'</param>
        public HostLogLevelDto(string host, LogEventLevel level)
        {
            Host = host;
            Level = level;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "host")]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'Verbose', 'Debug',
        /// 'Information', 'Warning', 'Error', 'Fatal'
        /// </summary>
        [JsonProperty(PropertyName = "level")]
        public LogEventLevel Level { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Host == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Host");
            }
        }
    }
}
