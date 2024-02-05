using System.Runtime.Serialization;
using System.Security;

namespace WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults
{
    public interface ILexisNexisBridgerKeyVault
    {
        [DataMember(Name = "lexisNexisBridgerClientId")]
        SecureString ClientId { get; set; }

        [DataMember(Name = "lexisNexisBridgerRolesOrUsers")]
        SecureString RolesOrUsers { get; set; }

        [DataMember(Name = "lexisNexisBridgerUserID")]
        SecureString UserId { get; set; }
    }
}
