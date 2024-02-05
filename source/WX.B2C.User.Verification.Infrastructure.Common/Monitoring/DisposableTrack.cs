using System;
using System.Diagnostics;
using System.Net;

namespace WX.B2C.User.Verification.Infrastructure.Common.Monitoring
{
    internal partial class PrometheusMetricsLogger
    {
        private class DisposableTrack : IDisposable
        {
            private readonly bool _isServiceRequest;
            private readonly PrometheusMetricsLogger _logger;
            private readonly string _name;
            private readonly string _protocol;
            private readonly Stopwatch _sw;
            private readonly string _target;
            private readonly string _type;
            private bool _disposed;

            public DisposableTrack(PrometheusMetricsLogger logger,
                                   string target,
                                   string protocol,
                                   string type,
                                   string name,
                                   bool isServiceRequest)
            {
                _logger = logger;
                _target = target;
                _protocol = protocol;
                _type = type;
                _name = name;
                _isServiceRequest = isServiceRequest;
                if (isServiceRequest)
                    _logger._dashboard.GenericConcurrentRequests.Inc();
                else
                    _logger._dashboard.GenericConcurrentDependencies.Inc();

                _logger._dashboard.SpecificConcurrentOperations.WithLabels(_name).Inc();
                _sw = new Stopwatch();
                _sw.Start();
            }

            public Exception Exception { get; set; }

            public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                _sw.Stop();
                var elapsedMilliseconds = _sw.ElapsedMilliseconds == 0 ? 1 : _sw.ElapsedMilliseconds;
                var dashboard = _logger._dashboard;

                if (Exception != null)
                {
                    if (_isServiceRequest)
                        dashboard.GenericTotalFailedOperations.WithLabels(StatusCode.ToString(), Exception.GetType().Name).Observe(1);
                    else
                        dashboard.GenericTotalFailedDependencies.WithLabels(StatusCode.ToString(), Exception.GetType().Name).Observe(1);

                    dashboard.GenericFailedResponseTime.WithLabels(StatusCode.ToString(), Exception.GetType().Name)
                                            .Observe(elapsedMilliseconds);
                    dashboard.GenericFailedDetailedResponseTime.WithLabels(StatusCode.ToString(), Exception.GetType().Name)
                                            .Observe(elapsedMilliseconds);

                    dashboard.SpecificFailedRequests
                                            .WithLabels(_target, _protocol, _type, _name, StatusCode.ToString(), Exception.GetType().Name)
                                            .Observe(1);
                    dashboard.SpecificFailedResponseTime
                                            .WithLabels(_target, _protocol, _type, _name, StatusCode.ToString(), Exception.GetType().Name)
                                            .Observe(elapsedMilliseconds);
                    dashboard.SpecificFailedDetailedResponseTime
                                            .WithLabels(_target, _protocol, _type, _name, StatusCode.ToString(), Exception.GetType().Name)
                                            .Observe(elapsedMilliseconds);
                }
                else
                {
                    if (_isServiceRequest)
                        dashboard.GenericTotalSuccessOperations.WithLabels(StatusCode.ToString()).Observe(1);
                    else
                        dashboard.GenericTotalSuccessDependencies.WithLabels(StatusCode.ToString()).Observe(1);

                    dashboard.GenericSuccessResponseTime.WithLabels(StatusCode.ToString()).Observe(elapsedMilliseconds);
                    dashboard.GenericSuccessDetailedResponseTime.WithLabels(StatusCode.ToString()).Observe(elapsedMilliseconds);

                    dashboard.SpecificSuccessRequests.WithLabels(_target, _protocol, _type, _name, StatusCode.ToString()).Observe(1);
                    dashboard.SpecificSuccessResponseTime.WithLabels(_target, _protocol, _type, _name, StatusCode.ToString())
                                            .Observe(elapsedMilliseconds);
                    dashboard.SpecificSuccessDetailedResponseTime.WithLabels(_target, _protocol, _type, _name, StatusCode.ToString())
                                            .Observe(elapsedMilliseconds);
                }

                dashboard.SpecificConcurrentOperations.WithLabels(_name).Dec();
                if (_isServiceRequest)
                    dashboard.GenericConcurrentRequests.Dec();
                else
                    dashboard.GenericConcurrentDependencies.Dec();
            }
        }
    }
}