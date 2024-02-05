using System.Runtime.Serialization;
using System.Security;

namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure.EventsService;

public interface IEventsKeyVault
{
    [DataMember(Name = "eventHubNameSpaceConnectionString.*")]
    Dictionary<string, SecureString> EventHubNameSpaceConnectionStrings { get; }

    [DataMember(Name = "b2cStorageConnectionString")]
    SecureString StorageConnectionString { get; }

    [DataMember(Name = "eventHubPrivateKey")]
    SecureString EventHubPrivateKey { get; }

    [DataMember(Name = "eventHubPublicKey")]
    SecureString EventHubPublicKey { get; }
}