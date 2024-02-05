using System;
using System.Collections.Generic;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using WX.B2C.User.Verification.Infrastructure.Remoting.FabricWrappers;
using WX.B2C.User.Verification.Infrastructure.Remoting.FabricWrappers.Stateless;

namespace WX.B2C.User.Verification.Infrastructure.Remoting.Attributes
{
    public class WxFabricTransportServiceRemotingProviderAttribute : FabricTransportServiceRemotingProviderAttribute
    {
        public override Dictionary<string, Func<ServiceContext, IService, IServiceRemotingListener>> CreateServiceRemotingListeners()
        {
            return new Dictionary<string, Func<ServiceContext, IService, IServiceRemotingListener>>
            {
                {
                    "V2Listener",
                    CreateListener
                }
            };
        }

        public override IServiceRemotingClientFactory CreateServiceRemotingClientFactoryV2(
            IServiceRemotingCallbackMessageHandler callbackMessageHandler)
        {
            var operationContextProvider = new OperationContextProvider();
            var inner = base.CreateServiceRemotingClientFactoryV2(callbackMessageHandler);
            return new ServiceRemotingClientFactoryWrapper(inner, operationContextProvider);
        }

        private static IServiceRemotingListener CreateListener(ServiceContext serviceContext, IService serviceImplementation)
        {
            var operationContextProvider = new OperationContextProvider();
            var messageHandler = new ServiceRemotingDispatcherWrapper(serviceContext, serviceImplementation, operationContextProvider);
            return new FabricTransportServiceRemotingListener(serviceContext, messageHandler);
        }
    }
}
