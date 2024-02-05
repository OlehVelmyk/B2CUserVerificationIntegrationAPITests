using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;

namespace WX.B2C.User.Verification.Infrastructure.Contracts.KeyVaults
{
    public interface IMessagingKeyVault
    {
        [DataMember(Name = "eventHubNameSpaceConnectionString.*")]
        Dictionary<string, SecureString> EventHubNameSpaceConnectionStrings { get; }

        [DataMember(Name = "storageConnectionString")]
        SecureString StorageConnectionString { get; }

        [DataMember(Name = "b2cStorageConnectionString")]
        SecureString B2CStorageConnectionString { get; }

        [DataMember(Name = "eventHubPrivateKey")]
        SecureString EventHubPrivateKey { get; }

        [DataMember(Name = "eventHubPublicKey")]
        SecureString EventHubPublicKey { get; }
    }
}
