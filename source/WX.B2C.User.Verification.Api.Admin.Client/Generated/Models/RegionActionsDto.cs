// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Api.Admin.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class RegionActionsDto
    {
        /// <summary>
        /// Initializes a new instance of the RegionActionsDto class.
        /// </summary>
        public RegionActionsDto()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the RegionActionsDto class.
        /// </summary>
        /// <param name="regionType">Possible values include: 'Global',
        /// 'Region', 'Country', 'State'</param>
        public RegionActionsDto(RegionType regionType, string region, IList<ActionDto> actions)
        {
            RegionType = regionType;
            Region = region;
            Actions = actions;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'Global', 'Region',
        /// 'Country', 'State'
        /// </summary>
        [JsonProperty(PropertyName = "regionType")]
        public RegionType RegionType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "region")]
        public string Region { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "actions")]
        public IList<ActionDto> Actions { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Region == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Region");
            }
            if (Actions == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Actions");
            }
            if (Actions != null)
            {
                foreach (var element in Actions)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
        }
    }
}
