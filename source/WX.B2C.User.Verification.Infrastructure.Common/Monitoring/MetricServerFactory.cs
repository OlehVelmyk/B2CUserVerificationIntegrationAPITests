using Prometheus;

namespace WX.B2C.User.Verification.Infrastructure.Common.Monitoring
{
    /// <summary>
    /// Due to using autofac and problem with double containers in hosts we have to implement classic singleton.
    /// </summary>
    internal static class MetricServerFactory
    {
        private static readonly object _lockObject = new();
        private static IMetricServer _metricServer;

        public static IMetricServer Start(int port)
        {
            if (_metricServer == null)
            {
                lock (_lockObject)
                {
                    if (_metricServer == null)
                    {
                        var metricServer = new MetricServer(port);
                        try
                        {
                            _metricServer = metricServer.Start();
                        }
                        catch
                        {
                            _metricServer = metricServer;
                        }
                    }
                }
            }
            return _metricServer;
        }
    }
}