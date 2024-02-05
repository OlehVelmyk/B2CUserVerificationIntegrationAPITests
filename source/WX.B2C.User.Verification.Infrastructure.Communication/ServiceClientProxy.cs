using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Communication
{

    internal interface IServiceClientProxy<TService>
    {
        Task Execute(Func<TService, Task> func);

        Task<T> Execute<T>(Func<TService, Task<T>> func);
    }

    internal class ServiceClientProxy<TService>
        : IServiceClientProxy<TService>
    {
        private readonly TService _proxy;
        private readonly IMetricsLogger _metricsLogger;

        private ServiceClientProxy(TService proxy, IMetricsLogger metricsLogger)
        {
            _proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
            _metricsLogger = metricsLogger ?? throw new ArgumentNullException(nameof(metricsLogger));
        }

        public static ServiceClientProxy<TService> Wrap(TService proxy, IMetricsLogger metricsLogger) => 
            new(proxy, metricsLogger);

        public async Task Execute(Func<TService, Task> func)
        {
            try
            {
                await _metricsLogger.ExecuteWithRpcRequestTracking(() => func(_proxy));
            }
            catch (AggregateException e) when(e.InnerExceptions.Count == 1)
            {
                ExceptionDispatchInfo.Capture(e.GetBaseException()).Throw();
            }
        }

        public async Task<T> Execute<T>(Func<TService, Task<T>> func)
        {
            try
            {
                return await _metricsLogger.ExecuteWithRpcRequestTracking(() => func(_proxy));
            }
            catch (AggregateException e) when (e.InnerExceptions.Count == 1)
            {
                ExceptionDispatchInfo.Capture(e.GetBaseException()).Throw();
                throw;
            }
        }
    }
}