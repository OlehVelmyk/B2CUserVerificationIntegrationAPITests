using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace WX.B2C.User.Verification.Infrastructure.Remoting.FabricWrappers.Stateless
{
    internal class ServiceRemotingDispatcherWrapper : ServiceRemotingMessageDispatcher
    {
        private readonly IOperationContextSetter _contextSetter;

        public ServiceRemotingDispatcherWrapper(IEnumerable<Type> remotingTypes,
                                                ServiceContext serviceContext,
                                                object serviceImplementation,
                                                IOperationContextSetter contextSetter,
                                                IServiceRemotingMessageBodyFactory serviceRemotingMessageBodyFactory = null)
            : base(remotingTypes,
                serviceContext,
                serviceImplementation,
                serviceRemotingMessageBodyFactory)
        {
            _contextSetter = contextSetter ?? throw new ArgumentNullException(nameof(contextSetter));
        }

        public ServiceRemotingDispatcherWrapper(ServiceContext serviceContext,
                                                IService serviceImplementation,
                                                IOperationContextSetter contextSetter,
                                                IServiceRemotingMessageBodyFactory serviceRemotingMessageBodyFactory = null)
            : base(serviceContext,
                serviceImplementation,
                serviceRemotingMessageBodyFactory)
        {
            _contextSetter = contextSetter ?? throw new ArgumentNullException(nameof(contextSetter));
        }

        public override void HandleOneWayMessage(IServiceRemotingRequestMessage requestMessage)
        {
            ReadHeaders(requestMessage);
            base.HandleOneWayMessage(requestMessage);
        }

        public override Task<IServiceRemotingResponseMessage> HandleRequestResponseAsync(
            IServiceRemotingRequestContext requestContext,
            IServiceRemotingRequestMessage requestMessage)
        {
            ReadHeaders(requestMessage);
            return base.HandleRequestResponseAsync(requestContext, requestMessage);
        }

        private void ReadHeaders(IServiceRemotingRequestMessage requestMessage)
        {
            var context = requestMessage.ToOperationContext();
            _contextSetter.SetContext(context);
        }
    }
}
