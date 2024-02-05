using System;
using Serilog.Core;
using Serilog.Events;
using WX.B2C.User.Verification.Infrastructure.Contracts;
using Constants = WX.B2C.User.Verification.Infrastructure.Remoting.Constants;

namespace WX.B2C.User.Verification.Infrastructure.Common.Logging
{
    internal class OperationContextEnricher : ILogEventEnricher
    {
        private readonly IOperationContextProvider _contextProvider;

        public OperationContextEnricher(IOperationContextProvider contextProvider)
        {
            _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var context = _contextProvider.GetContextOrDefault();

            AddPropertyIfNotEmpty(context.CorrelationId, Constants.Headers.CorrelationId);
            AddPropertyIfNotEmpty(context.CorrelationId, Constants.Headers.Reference);
            AddPropertyIfNotEmpty(context.OperationId, Constants.Headers.OperationId);
            AddPropertyIfNotEmpty(context.OperationName, Constants.Headers.OperationName);
            AddPropertyIfNotEmpty(context.ParentOperationId, Constants.Headers.ParentOperationId);

            void AddPropertyIfNotEmpty<T>(T property, string propertyName)
            {
                if (Equals(property, default(T)))
                    return;

                var eventProperty = propertyFactory.CreateProperty(propertyName, property);
                logEvent.AddOrUpdateProperty(eventProperty);
            }
        }
    }
}
