// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace WX.B2C.User.Verification.Onfido.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class IdentityEnhancedAddressBreakdown
    {
        /// <summary>
        /// Initializes a new instance of the IdentityEnhancedAddressBreakdown
        /// class.
        /// </summary>
        public IdentityEnhancedAddressBreakdown()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the IdentityEnhancedAddressBreakdown
        /// class.
        /// </summary>
        /// <param name="telephoneDatabase">Address match against telephone
        /// database.</param>
        /// <param name="votingRegister">Address match against voting
        /// register.</param>
        public IdentityEnhancedAddressBreakdown(IdentityEnhancedCreditAgencies creditAgencies = default(IdentityEnhancedCreditAgencies), DefaultBreakdownResult telephoneDatabase = default(DefaultBreakdownResult), DefaultBreakdownResult votingRegister = default(DefaultBreakdownResult))
        {
            CreditAgencies = creditAgencies;
            TelephoneDatabase = telephoneDatabase;
            VotingRegister = votingRegister;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "credit_agencies")]
        public IdentityEnhancedCreditAgencies CreditAgencies { get; set; }

        /// <summary>
        /// Gets or sets address match against telephone database.
        /// </summary>
        [JsonProperty(PropertyName = "telephone_database")]
        public DefaultBreakdownResult TelephoneDatabase { get; set; }

        /// <summary>
        /// Gets or sets address match against voting register.
        /// </summary>
        [JsonProperty(PropertyName = "voting_register")]
        public DefaultBreakdownResult VotingRegister { get; set; }

    }
}
