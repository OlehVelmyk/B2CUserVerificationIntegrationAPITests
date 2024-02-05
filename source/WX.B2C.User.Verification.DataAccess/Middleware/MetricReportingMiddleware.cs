using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.DataAccess.Middleware
{
    public class MetricReportingMiddleware : IInterceptor
    {
        private readonly IMetricsLogger _metricsLogger;

        public MetricReportingMiddleware(IMetricsLogger metricsLogger)
        {
            _metricsLogger = metricsLogger ?? throw new ArgumentNullException(nameof(metricsLogger));
        }

        public void Intercept(IInvocation invocation) =>
            _metricsLogger.ExecuteWithDatabaseDependencyTracking( 
                invocation.TargetType.Name,
                invocation.Method.Name,
                () =>
                {
                    invocation.Proceed();
                    return invocation.ReturnValue as Task ?? Task.CompletedTask;
                });
    }
}
