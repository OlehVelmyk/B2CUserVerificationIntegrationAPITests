using System;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Communication
{
    internal interface IServiceClientFactory
    {
        IServiceClientProxy<TService> CreateStatefulServiceProxy<TService>(Uri serviceUri, ServicePartitionKey partitionKey)
            where TService : IService;

        IServiceClientProxy<TService> CreateStatefulServiceProxy<TService>(Uri serviceUri) where TService : IService;

        IServiceClientProxy<TService> CreateStatelessServiceProxy<TService>(Uri serviceUri) where TService : IService;

        IServiceClientProxy<TActorInterface> CreateActorProxy<TActorInterface>(ActorId actorId, Uri serviceUri)
            where TActorInterface : IActor;

        IServiceClientProxy<TActorInterface> CreateActorProxy<TActorInterface>(Guid actorId, Uri serviceUri)
            where TActorInterface : IActor;
    }

    internal class ServiceClientFactory : IServiceClientFactory
    {
        private readonly IMetricsLogger _metricsLogger;

        public ServiceClientFactory(IMetricsLogger metricsLogger)
        {
            _metricsLogger = metricsLogger ?? throw new ArgumentNullException(nameof(metricsLogger));
        }

        public IServiceClientProxy<TService> CreateStatefulServiceProxy<TService>(Uri serviceUri, ServicePartitionKey partitionKey)
            where TService : IService =>
            ServiceClientProxy<TService>.Wrap(ServiceProxy.Create<TService>(serviceUri, partitionKey), _metricsLogger);

        public IServiceClientProxy<TService> CreateStatefulServiceProxy<TService>(Uri serviceUri) where TService : IService
        {
            var byteArray = Guid.NewGuid().ToByteArray();
            var partitionKey = BitConverter.ToInt64(byteArray, 0) ^ BitConverter.ToInt64(byteArray, 8);
            return ServiceClientProxy<TService>.Wrap(ServiceProxy.Create<TService>(serviceUri, new ServicePartitionKey(partitionKey)),
                                                     _metricsLogger);
        }

        public IServiceClientProxy<TService> CreateStatelessServiceProxy<TService>(Uri serviceUri) where TService : IService =>
            ServiceClientProxy<TService>.Wrap(ServiceProxy.Create<TService>(serviceUri), _metricsLogger);

        public IServiceClientProxy<TActorInterface> CreateActorProxy<TActorInterface>(ActorId actorId, Uri serviceUri)
            where TActorInterface : IActor =>
            ServiceClientProxy<TActorInterface>.Wrap(ActorProxy.Create<TActorInterface>(actorId, serviceUri), _metricsLogger);

        public IServiceClientProxy<TActorInterface> CreateActorProxy<TActorInterface>(Guid actorId, Uri serviceUri)
            where TActorInterface : IActor =>
            CreateActorProxy<TActorInterface>(new ActorId(actorId), serviceUri);
    }
}