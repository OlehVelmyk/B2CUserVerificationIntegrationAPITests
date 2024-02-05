using Microsoft.ServiceFabric.Services.Remoting;
using WX.B2C.User.Verification.Infrastructure.Remoting.Attributes;

[assembly: WxFabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2, ConnectTimeoutInMilliseconds = 20000)]
[assembly: WxFabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2, ConnectTimeoutInMilliseconds = 20000)]