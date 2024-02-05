using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;
using WX.Messaging.Subscriber.EventHub.Interfaces;

namespace WX.B2C.User.Verification.Listener.PassFort
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class EventListenerService : StatelessService
    {
        private readonly IEventHubSubscriberRunner _runner;

        public EventListenerService(StatelessServiceContext context, IEventHubSubscriberRunner runner)
            : base(context)
        {
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }

        protected override Task RunAsync(CancellationToken cancellationToken) => _runner.RunAsync(cancellationToken);
    }
}
