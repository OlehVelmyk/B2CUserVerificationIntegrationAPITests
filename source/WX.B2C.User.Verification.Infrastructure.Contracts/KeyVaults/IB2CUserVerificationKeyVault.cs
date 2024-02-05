using System.Runtime.Serialization;
using System.Security;

namespace WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults
{
    public interface IB2CUserVerificationKeyVault
    {
        [DataMember(Name = "b2cUserVerificationDBConnectionString")]
        SecureString DbConnectionString { get; }

        [DataMember(Name = "splunkEndpoint")]
        SecureString SplunkEndpoint { get; }

        [DataMember(Name = "splunkToken")]
        SecureString SplunkToken { get; }

        [DataMember(Name = "appInsightsInstrumentationKey")]
        SecureString AppInsightsInstrumentationKey { get; }

        [DataMember(Name = "ipstackToken")]
        SecureString IpStackToken { get; }

        [DataMember(Name = "onfidoApiToken")]
        SecureString OnfidoApiToken { get; }
        
        [DataMember(Name = "b2cStorageConnectionString")]
        SecureString B2CStorageConnectionString { get; }
    }
}
