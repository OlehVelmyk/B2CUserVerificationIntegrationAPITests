using System.Runtime.Serialization;
using System.Security;

namespace WX.B2C.User.Verification.Component.Tests
{
    public interface ITestKeyVault
    {
        [DataMember(Name = "adminPanelApplicationCertificate")]
        SecureString AdminPanelApplicationCertificate { get; }
    }
}