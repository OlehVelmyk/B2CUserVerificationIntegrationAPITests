using Serilog;
using WX.Core.TypeExtensions;
using WX.Messaging.Core;
using WX.Messaging.EventHub;
using WX.Messaging.EventHub.Management;
using WX.Messaging.EventHub.Models;
using WX.Messaging.Publisher;
using WX.Messaging.Publisher.EventHub;
using WX.Messaging.Publisher.EventHub.EventHubClients;
using WX.Messaging.Publisher.Services;

namespace WX.B2C.User.Verification.Integration.Tests.Infrastructure.EventsService;

public class EventPublisher
{
    private readonly IEventPublisher _publisher;

    public EventPublisher(IEventsKeyVault keyVault, ILogger logger)
    {
        var eventHubConfig = new EventHubConfig
        {
            StorageConnectionString = keyVault.StorageConnectionString.UnSecure(),
            EventHubConnectionStrings = keyVault.EventHubNameSpaceConnectionStrings.OrderBy(p => p.Key)
                .Select(p => p.Value.UnSecure())
                .ToArray(),
            ServiceName = "backend",
            PrivateKeyProvider = () => keyVault.EventHubPrivateKey.UnSecure()
        };

        var rootConnectionStringResolver = new RootConnectionStringResolver(eventHubConfig, new AzureManagementProviderFactory(logger), logger);
        var eventHubClientFactory = new EventHubClientFactory(logger, rootConnectionStringResolver, new PublisherChannelNameResolver(), eventHubConfig);
        _publisher = new EventHubPublisher(logger, eventHubClientFactory);
    }

    public Task PublishAsync<T>(T message) where T : Event => _publisher.PublishAsync(message);
}