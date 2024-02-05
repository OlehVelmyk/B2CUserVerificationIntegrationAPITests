using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Autofac;
using WX.Logging;
using WX.Logging.Autofac;
using WX.Messaging.Core;
using WX.Messaging.EventHub.Interfaces;
using WX.Messaging.EventHub.Models;
using WX.Messaging.Stub.Autofac;
using WX.Messaging.Subscriber.Autofac;
using WX.Messaging.Subscriber.EventHub.Interfaces;
using WX.Messaging.Subscriber.HandlerResolving;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures.Events
{
    internal class EventStorage : IDisposable
    {
        private static readonly Lazy<EventStorage> LazyInitialization = new (() => new EventStorage());
        private readonly GlobalEventHandler _globalHandler;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private EventStorage()
        {
            Debug.WriteLine($"{nameof(EventStorage)} create {Guid.NewGuid()}");

            var builder = new ContainerBuilder();
            builder.RegisterLogging((_, configuration) => configuration.WriteToDebug());
            builder.RegisterEventHubSubscriptionSupport(ResolveConfig, configurationBuilder => configurationBuilder.WithStub(_ => true));
            builder.RegisterEventHandler<GlobalEventHandler>();
            var container = builder.Build();

            _globalHandler = container.Resolve<IEventHandler>() as GlobalEventHandler;

            _cancellationTokenSource = new CancellationTokenSource();

            var runner = container.Resolve<IEventHubSubscriberRunner>();
            runner.RunAsync(_cancellationTokenSource.Token);
        }

        ~EventStorage()
        {
            Dispose(false);
        }

        public static EventStorage Instance => LazyInitialization.Value;

        private static IEventHubConfig ResolveConfig(IComponentContext context)
        {
            return new EventHubConfig
            {
                ServiceName = "WX.B2C.User.Verification.EventStorage",
            };
        }

        public IEnumerable<TEvent> GetAllEvents<TEvent>() where TEvent : Event
        {
            return _globalHandler.GetAllEvents<TEvent>();
        }

        private void Dispose(bool disposing)
        {
            Debug.WriteLine($"{nameof(EventsFixture)} dispose {Guid.NewGuid()}");
            if (!disposing)
                return;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
