using System;
using System.Fabric;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Remoting.FabricWrappers
{
    internal class ServiceRemotingClientWrapper : IServiceRemotingClient
    {
        private readonly IOperationContextProvider _contextProvider;
        public readonly IServiceRemotingClient Inner;

        public ServiceRemotingClientWrapper(
            IServiceRemotingClient inner,
            IOperationContextProvider contextProvider)
        {
            Inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public ResolvedServicePartition ResolvedServicePartition
        {
            get => Inner.ResolvedServicePartition;
            set => Inner.ResolvedServicePartition = value;
        }

        public string ListenerName
        {
            get => Inner.ListenerName;
            set => Inner.ListenerName = value;
        }

        public ResolvedServiceEndpoint Endpoint
        {
            get => Inner.Endpoint;
            set => Inner.Endpoint = value;
        }

        public Task<IServiceRemotingResponseMessage> RequestResponseAsync(IServiceRemotingRequestMessage requestMessage)
        {
            SetHeaders(requestMessage);
            return Inner.RequestResponseAsync(requestMessage);
        }

        public void SendOneWay(IServiceRemotingRequestMessage requestMessage)
        {
            SetHeaders(requestMessage);
            Inner.RequestResponseAsync(requestMessage);
        }

        private void SetHeaders(IServiceRemotingRequestMessage requestMessage)
        {
            var context = _contextProvider.GetContextOrDefault();
            requestMessage.PopulateFromOperationContext(context);
        }
    }
}
