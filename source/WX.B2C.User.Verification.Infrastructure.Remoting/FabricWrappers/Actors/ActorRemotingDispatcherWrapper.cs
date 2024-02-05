using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace WX.B2C.User.Verification.Infrastructure.Remoting.FabricWrappers.Actors
{
    internal class ActorRemotingDispatcherWrapper : ActorServiceRemotingDispatcher
    {
        private readonly IOperationContextSetter _contextSetter;

        public ActorRemotingDispatcherWrapper(
            ActorService actorService,
            IServiceRemotingMessageBodyFactory serviceRemotingRequestMessageBodyFactory,
            IOperationContextSetter contextSetter)
            : base(actorService, serviceRemotingRequestMessageBodyFactory)
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
            var operationContext = requestMessage.ToOperationContext();
            _contextSetter.SetContext(operationContext);
        }
    }
}
