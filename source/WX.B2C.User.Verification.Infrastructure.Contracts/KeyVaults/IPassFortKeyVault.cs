using System.Runtime.Serialization;
using System.Security;

namespace WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults
{
    public interface IPassFortKeyVault
    {
        [DataMember(Name = "passFortApiKey")]
        SecureString PassFortApiKey { get; }

        [DataMember(Name = "passFortAPACApplicationProductId")]
        SecureString ApacApplicationProductId { get; }

        [DataMember(Name = "passFortROTWApplicationProductId")]
        SecureString RowApplicationProductId { get; }

        [DataMember(Name = "passFortSecret")]
        SecureString PassFortSecret { get; }
    }
}
