using System;
using Serilog.Context;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Remoting
{
    public interface IOperationContextScopeFactory
    {
        IDisposable Create(Guid correlationId, Guid? parentOperationId, string operationName);
    }

    internal class OperationContextScopeFactory : IOperationContextScopeFactory
    {
        private readonly IOperationContextSetter _contextSetter;

        public OperationContextScopeFactory(IOperationContextSetter contextSetter)
        {
            _contextSetter = contextSetter ?? throw new ArgumentNullException(nameof(contextSetter));
        }
        
        public IDisposable Create(Guid correlationId, Guid? parentOperationId, string operationName)
        {
            var providedEmptyCorrelation = correlationId == Guid.Empty;
            if (providedEmptyCorrelation)
                correlationId = Guid.NewGuid();
            
            var operationContext = OperationContext.Create(
                correlationId,
                parentOperationId,
                Guid.NewGuid(),
                operationName
            );

            _contextSetter.SetContext(operationContext);

            return new ScopeContext(
            PushProperty(Constants.Headers.CorrelationId, correlationId),
            PushProperty(Constants.Headers.OperationName, operationName),
            PushProperty(Constants.Headers.ParentOperationId, parentOperationId),
            PushProperty(Constants.Headers.OperationId, parentOperationId),
            PushProperty(Constants.Headers.ProvidedEmptyCorrelation, providedEmptyCorrelation));
            
            static IDisposable PushProperty<T>(string property, T value) =>
                Equals(value, default(T)) ? null : LogContext.PushProperty(property, value);
        }

        private class ScopeContext : IDisposable
        {
            private readonly IDisposable[] _logContext;

            public ScopeContext(params IDisposable[] logContext)
            {
                _logContext = logContext;
            }

            public void Dispose()
            {
                foreach (var logContext in _logContext)
                {
                    logContext?.Dispose();
                }
            }
        }
    }
}
