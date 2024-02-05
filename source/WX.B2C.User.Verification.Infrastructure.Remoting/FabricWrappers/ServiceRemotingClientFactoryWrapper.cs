using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Remoting.FabricWrappers
{
    internal class ServiceRemotingClientFactoryWrapper : IServiceRemotingClientFactory
    {
        private readonly IServiceRemotingClientFactory _inner;
        private readonly IOperationContextProvider _contextProvider;

        public ServiceRemotingClientFactoryWrapper(
            IServiceRemotingClientFactory inner,
            IOperationContextProvider contextProvider)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>>
            ICommunicationClientFactory<IServiceRemotingClient>.ClientConnected
        {
            add => _inner.ClientConnected += value;
            remove => _inner.ClientConnected -= value;
        }

        event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>>
            ICommunicationClientFactory<IServiceRemotingClient>.ClientDisconnected
        {
            add => _inner.ClientDisconnected += value;
            remove => _inner.ClientDisconnected -= value;
        }

        public async Task<IServiceRemotingClient> GetClientAsync(Uri serviceUri,
                                                                 ServicePartitionKey partitionKey,
                                                                 TargetReplicaSelector targetReplicaSelector,
                                                                 string listenerName,
                                                                 OperationRetrySettings retrySettings,
                                                                 CancellationToken cancellationToken)
        {
            var getClient = _inner.GetClientAsync(serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings, cancellationToken);
            var client = await getClient.ConfigureAwait(false);
            return new ServiceRemotingClientWrapper(client, _contextProvider);
        }

        public async Task<IServiceRemotingClient> GetClientAsync(ResolvedServicePartition previousRsp,
                                                                 TargetReplicaSelector targetReplicaSelector,
                                                                 string listenerName,
                                                                 OperationRetrySettings retrySettings,
                                                                 CancellationToken cancellationToken)
        {
            var getClient = _inner.GetClientAsync(previousRsp, targetReplicaSelector, listenerName, retrySettings, cancellationToken);
            var client = await getClient.ConfigureAwait(false);
            return new ServiceRemotingClientWrapper(client, _contextProvider);
        }

        public Task<OperationRetryControl> ReportOperationExceptionAsync(IServiceRemotingClient client,
                                                                         ExceptionInformation exceptionInformation,
                                                                         OperationRetrySettings retrySettings,
                                                                         CancellationToken cancellationToken)
        {
            var clientWrapper = client as ServiceRemotingClientWrapper;
            var innerClient = clientWrapper.Inner;
            return _inner.ReportOperationExceptionAsync(innerClient, exceptionInformation, retrySettings, cancellationToken);
        }

        public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory() =>
            _inner.GetRemotingMessageBodyFactory();
    }
}
