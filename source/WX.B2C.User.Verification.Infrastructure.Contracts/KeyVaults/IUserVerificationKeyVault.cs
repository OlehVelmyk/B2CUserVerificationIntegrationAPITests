using System.Runtime.Serialization;
using System.Security;

namespace WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults
{
    public interface IUserVerificationKeyVault
    {
        [DataMember(Name = "userVerificationDbConnectionString")]
        SecureString DbConnectionString { get; }
    }
}
