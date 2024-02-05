using System.Runtime.Serialization;
using System.Security;

namespace WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults
{
    public interface IMigrationKeyVault
    {
        [DataMember(Name = "b2cUserVerificationDBConnectionString")]
        SecureString DbConnectionString { get; }
    }
}
