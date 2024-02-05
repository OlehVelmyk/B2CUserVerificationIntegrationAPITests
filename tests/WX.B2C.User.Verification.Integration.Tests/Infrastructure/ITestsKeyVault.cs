using System.Runtime.Serialization;
using System.Security;

namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure;

public interface ITestsKeyVault
{
    [DataMember(Name = "configurationStorageConnectionString")]
    SecureString ConfigurationStorageConnectionString { get; }

    [DataMember(Name = "externalUserProviderQueueConnectionString")]
    SecureString CommandsQueueConnectionString { get; }

    [DataMember(Name = "configurationBackupStorageConnectionString")]
    SecureString ConfigurationBackupStorageConnectionString { get; }
}