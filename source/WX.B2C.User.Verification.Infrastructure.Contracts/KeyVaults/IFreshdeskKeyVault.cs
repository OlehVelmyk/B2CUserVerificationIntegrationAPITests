using System.Runtime.Serialization;
using System.Security;

namespace WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults
{
    public interface IFreshdeskKeyVault
    {
        [DataMember(Name = "freshdeskApiKey")]
        SecureString ApiKey { get; }

        [DataMember(Name = "complianceFreshdeskAgentApiKey")]
        SecureString ComplianceAgentApiKey { get; }
        
        [DataMember(Name = "usaComplianceFreshdeskAgentApiKey")]
        SecureString USAComplianceAgentApiKey { get; }

        [DataMember(Name = "verificationFreshdeskAgentApiKey")]
        SecureString VerificationAgentApiKey { get; }
        
        [DataMember(Name = "usaVerificationFreshdeskAgentApiKey")]
        SecureString USAVerificationAgentApiKey { get; }
    }
}