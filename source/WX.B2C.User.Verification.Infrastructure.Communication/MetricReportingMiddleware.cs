using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Communication
{
    internal class MetricReportingMiddleware : IInterceptor
    {
        private readonly IMetricsLogger _metricsLogger;

        public MetricReportingMiddleware(IMetricsLogger  metricsLogger)
        {
            _metricsLogger = metricsLogger ?? throw new ArgumentNullException(nameof(metricsLogger));
        }

        public void Intercept(IInvocation invocation)
        {
            var operationName = $"{invocation.TargetType.Name}.{invocation.Method.Name}";
            _metricsLogger.ExecuteWithRpcRequestTracking(() =>
            {
                invocation.Proceed();
                return invocation.ReturnValue as Task ?? Task.CompletedTask;
            }, operationName);
        }
    }
}