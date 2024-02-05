using System;
using System.Collections.Generic;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Actors.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using WX.B2C.User.Verification.Infrastructure.Remoting.FabricWrappers;
using WX.B2C.User.Verification.Infrastructure.Remoting.FabricWrappers.Actors;

namespace WX.B2C.User.Verification.Infrastructure.Remoting.Attributes
{
    public class WxFabricTransportActorRemotingProviderAttribute : FabricTransportActorRemotingProviderAttribute
    {
        public override Dictionary<string, Func<ActorService, IServiceRemotingListener>> CreateServiceRemotingListeners()
        {
            return new Dictionary<string, Func<ActorService, IServiceRemotingListener>>
            {
                {
                    "V2Listener",
                    CreateRemotingListener
                }
            };
        }

        public override IServiceRemotingClientFactory CreateServiceRemotingClientFactory(
            IServiceRemotingCallbackMessageHandler callbackMessageHandler)
        {
            var operationContextProvider = new OperationContextProvider();
            var inner = base.CreateServiceRemotingClientFactory(callbackMessageHandler);
            return new ServiceRemotingClientFactoryWrapper(inner, operationContextProvider);
        }

        private static IServiceRemotingListener CreateRemotingListener(ActorService actorService)
        {
            var serializationProvider = new ServiceRemotingDataContractSerializationProvider();
            var messageBodyFactory = serializationProvider.CreateMessageBodyFactory();
            var operationContextProvider = new OperationContextProvider();
            var messageHandler = new ActorRemotingDispatcherWrapper(actorService, messageBodyFactory, operationContextProvider);
            return new FabricTransportActorServiceRemotingListener(actorService, messageHandler);
        }
    }
}
