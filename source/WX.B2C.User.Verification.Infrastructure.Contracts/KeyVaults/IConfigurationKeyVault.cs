using System.Runtime.Serialization;
using System.Security;

namespace WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults
{
    public interface IConfigurationKeyVault
    {
        [DataMember(Name = "userApplicationConfigurationConnectionString")]
        SecureString ApplicationConfigurationConnectionString { get; }

        [DataMember(Name = "configurationStorageConnectionString")]
        SecureString ConfigurationStorageConnectionString { get; }
        
        [DataMember(Name = "configurationBackupStorageConnectionString")]
        SecureString ConfigurationBackupStorageConnectionString { get; }
    }
}
