using System.Runtime.Serialization;
using System.Security;

namespace WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults
{
    public interface ILexisNexisRdpKeyVault
    {
        [DataMember(Name = "lexisNexisRdpApiKeyId")]
        SecureString ApiKeyId { get; }

        [DataMember(Name = "lexisNexisRdpApiSecretKey")]
        SecureString ApiSecretKey { get; }

        [DataMember(Name = "lexisNexisRdpAccountId")]
        SecureString AccountId { get; }

        [DataMember(Name = "lexisNexisRdpWorkflowName")]
        SecureString WorkflowName { get; set; }
    }
}
