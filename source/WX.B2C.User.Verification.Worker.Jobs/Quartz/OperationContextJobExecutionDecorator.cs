using System;
using System.Threading.Tasks;
using Quartz;
using WX.B2C.User.Verification.Infrastructure.Remoting;
using WX.B2C.User.Verification.Worker.Jobs.Extensions;

namespace WX.B2C.User.Verification.Worker.Jobs.Quartz
{
    internal sealed class OperationContextJobExecutionDecorator : IJob
    {
        private readonly IJob _inner;
        private readonly IOperationContextScopeFactory _operationContextScopeFactory;

        public OperationContextJobExecutionDecorator(IJob inner, IOperationContextScopeFactory operationContextScopeFactory)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _operationContextScopeFactory = operationContextScopeFactory ?? throw new ArgumentNullException(nameof(operationContextScopeFactory));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // TODO: Review create context for JobSchedulerInitializer.
            var operationContext = context.GetOperationContext();
            using var operationContextScopeFactory = _operationContextScopeFactory.Create(operationContext.CorrelationId,
                                                                                                   operationContext.OperationId,
                                                                                                   operationContext.OperationName);
            await _inner.Execute(context);
        }
    }
}