using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using WX.B2C.User.Verification.Infrastructure.Remoting;
using WX.Messaging.Core;

namespace WX.B2C.User.Verification.Facade.EventHandlers
{
    public class EventHandlingContext
    {
        public EventHandlingContext(ILogger logger, IOperationContextScopeFactory contextScopeFactory, IMetricsLogger metricsLogger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ContextScopeFactory = contextScopeFactory ?? throw new ArgumentNullException(nameof(contextScopeFactory));
            MetricsLogger = metricsLogger ?? throw new ArgumentNullException(nameof(metricsLogger));
        }

        public ILogger Logger { get; }

        public IOperationContextScopeFactory ContextScopeFactory { get; }

        public IMetricsLogger MetricsLogger { get; }
    }

    internal class BaseEventHandler
    {
        private readonly IOperationContextScopeFactory _contextScopeFactory;
        private readonly IMetricsLogger _metricsLogger;

        protected BaseEventHandler(EventHandlingContext eventHandlingContext)
        {
            _contextScopeFactory = eventHandlingContext.ContextScopeFactory ?? throw new ArgumentNullException(nameof(eventHandlingContext.ContextScopeFactory));
            _metricsLogger = eventHandlingContext.MetricsLogger ?? throw new ArgumentNullException(nameof(eventHandlingContext.MetricsLogger));
            Logger = eventHandlingContext.Logger?.ForContext(GetType()) ?? throw new ArgumentNullException(nameof(eventHandlingContext.Logger));
        }

        protected ILogger Logger { get; }

        protected async Task Handle<TEventArgs>(Event<TEventArgs> @event, Func<TEventArgs, Task> processor)
            where TEventArgs : EventArgs
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var correlationId = @event.CorrelationId;
            var causationId = @event.CausationId;
            var operationName = $"{GetType().Name}.{@event.GetType()}";
            using var context = _contextScopeFactory.Create(correlationId, causationId, operationName);

            var sw = Stopwatch.StartNew();
            try
            {
                Logger.Information("{MessageType} started processing {@Event}", @event.GetType().Name,
                                   @event);

                await _metricsLogger.ExecuteWithEventHandlingTracking(() => processor(@event.EventArgs));

                sw.Stop();
                Logger.Information("{MessageType} finished processing {@Event}, elapsed: {Elapsed}",
                                   @event.GetType().Name, @event, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "{MessageType} error processing message: {@Event}", @event.GetType().Name, @event);
                throw;
            }
        }

    }
}