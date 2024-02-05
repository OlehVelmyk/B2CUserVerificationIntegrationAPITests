using System;
using Prometheus;

namespace WX.B2C.User.Verification.Infrastructure.Common.Monitoring
{
    public interface IPrometheusMetricsDashboard
    {
        Gauge GenericConcurrentDependencies { get; }

        Gauge GenericConcurrentRequests { get; }

        Histogram GenericFailedDetailedResponseTime { get; }

        Histogram GenericSuccessDetailedResponseTime { get; }

        Summary GenericFailedResponseTime { get; }

        Summary GenericSuccessResponseTime { get; }

        Summary GenericTotalFailedDependencies { get; }

        Summary GenericTotalFailedOperations { get; }

        Summary GenericTotalSuccessDependencies { get; }

        Summary GenericTotalSuccessOperations { get; }

        Gauge SpecificConcurrentOperations { get; }

        Histogram SpecificFailedDetailedResponseTime { get; }

        Histogram SpecificSuccessDetailedResponseTime { get; }

        Summary SpecificFailedRequests { get; }

        Summary SpecificFailedResponseTime { get; }

        Summary SpecificSuccessRequests { get; }

        Summary SpecificSuccessResponseTime { get; }
    }

    internal class PrometheusMetricsDashboard : IPrometheusMetricsDashboard
    {
        private const string OperationTargetLabel = "OperationTarget";
        private const string OperationProtocolLabel = "OperationProtocol";
        private const string OperationTargetTypeLabel = "OperationTargetType";
        private const string OperationNameLabel = "OperationName";
        private const string StatusLabel = "StatusCode";
        private const string ErrorTypeLabel = "ExceptionType";

        public Gauge GenericConcurrentDependencies { get; }

        public Gauge GenericConcurrentRequests { get; }

        public Histogram GenericFailedDetailedResponseTime { get; }

        public Histogram GenericSuccessDetailedResponseTime { get; }

        public Summary GenericFailedResponseTime { get; }

        public Summary GenericSuccessResponseTime { get; }

        public Summary GenericTotalFailedDependencies { get; }

        public Summary GenericTotalFailedOperations { get; }

        public Summary GenericTotalSuccessDependencies { get; }

        public Summary GenericTotalSuccessOperations { get; }

        public Gauge SpecificConcurrentOperations { get; }

        public Histogram SpecificFailedDetailedResponseTime { get; }

        public Histogram SpecificSuccessDetailedResponseTime { get; }

        public Summary SpecificFailedRequests { get; }

        public Summary SpecificFailedResponseTime { get; }

        public Summary SpecificSuccessRequests { get; }

        public Summary SpecificSuccessResponseTime { get; }

        public PrometheusMetricsDashboard(PrometheusMetricsFactory prometheusMetricsFactory)
        {
            if (prometheusMetricsFactory == null)
                throw new ArgumentNullException(nameof(prometheusMetricsFactory));

            GenericConcurrentDependencies =
                prometheusMetricsFactory.Gauge("Generic:ConcurrentDependencies", "Number of concurrent dependencies");
            GenericConcurrentRequests = prometheusMetricsFactory.Gauge("Generic:ConcurrentRequests", "Number of concurrent requests");
            GenericFailedDetailedResponseTime = prometheusMetricsFactory.Histogram("Generic:FailedResponseTimeHistogram",
                                                                                   "Average failed operation response time details",
                                                                                   StatusLabel,
                                                                                   ErrorTypeLabel);
            GenericFailedResponseTime = prometheusMetricsFactory.Summary("Generic:FailedResponseTimeSummary",
                                                                         "Average failed operation response time summary",
                                                                         false,
                                                                         StatusLabel,
                                                                         ErrorTypeLabel);
            GenericSuccessDetailedResponseTime = prometheusMetricsFactory.Histogram("Generic:SuccessResponseTimeHistogram",
                                                                                    "Average success operation response time details",
                                                                                    StatusLabel);
            GenericSuccessResponseTime = prometheusMetricsFactory.Summary("Generic:SuccessResponseTimeSummary",
                                                                          "Average success operation response time summary",
                                                                          false,
                                                                          StatusLabel);
            GenericTotalFailedDependencies = prometheusMetricsFactory.Summary("Generic:FailedDependencies",
                                                                              "Number of failed dependencies",
                                                                              false,
                                                                              StatusLabel,
                                                                              ErrorTypeLabel);
            GenericTotalFailedOperations =
                prometheusMetricsFactory.Summary("Generic:FailedOperations",
                                                 "Number of failed operations",
                                                 false,
                                                 StatusLabel,
                                                 ErrorTypeLabel);
            GenericTotalSuccessDependencies =
                prometheusMetricsFactory.Summary("Generic:SuccessDependencies", "Number of success dependencies", false, StatusLabel);
            GenericTotalSuccessOperations =
                prometheusMetricsFactory.Summary("Generic:SuccessOperations", "Number of success operations", false, StatusLabel);
            SpecificConcurrentOperations =
                prometheusMetricsFactory.Gauge("Specific:ConcurrentRequests",
                                               "Operation\\Dependency concurrent requests",
                                               OperationNameLabel);
            SpecificFailedDetailedResponseTime = prometheusMetricsFactory.Histogram("Specific:FailedResponseTimeHistogram",
                                                                                    "Failed Operation\\Dependency response time details",
                                                                                    OperationTargetLabel,
                                                                                    OperationProtocolLabel,
                                                                                    OperationTargetTypeLabel,
                                                                                    OperationNameLabel,
                                                                                    StatusLabel,
                                                                                    ErrorTypeLabel);
            SpecificFailedRequests = prometheusMetricsFactory.Summary("Specific:FailedOperations",
                                                                      "Failed Operation\\Dependency executions",
                                                                      false,
                                                                      OperationTargetLabel,
                                                                      OperationProtocolLabel,
                                                                      OperationTargetTypeLabel,
                                                                      OperationNameLabel,
                                                                      StatusLabel,
                                                                      ErrorTypeLabel);
            SpecificFailedResponseTime = prometheusMetricsFactory.Summary("Specific:FailedResponseTimeSummary",
                                                                          "Failed Operation\\Dependency response time summary",
                                                                          false,
                                                                          OperationTargetLabel,
                                                                          OperationProtocolLabel,
                                                                          OperationTargetTypeLabel,
                                                                          OperationNameLabel,
                                                                          StatusLabel,
                                                                          ErrorTypeLabel);
            SpecificSuccessDetailedResponseTime = prometheusMetricsFactory.Histogram("Specific:SuccessResponseTimeHistogram",
                                                                                     "Success Operation\\Dependency response time details",
                                                                                     OperationTargetLabel,
                                                                                     OperationProtocolLabel,
                                                                                     OperationTargetTypeLabel,
                                                                                     OperationNameLabel,
                                                                                     StatusLabel);
            SpecificSuccessRequests = prometheusMetricsFactory.Summary("Specific:SuccessOperations",
                                                                       "Success Operation\\Dependency executions",
                                                                       false,
                                                                       OperationTargetLabel,
                                                                       OperationProtocolLabel,
                                                                       OperationTargetTypeLabel,
                                                                       OperationNameLabel,
                                                                       StatusLabel);
            SpecificSuccessResponseTime = prometheusMetricsFactory.Summary("Specific:SuccessResponseTimeSummary",
                                                                           "Success Operation\\Dependency response time summary",
                                                                           false,
                                                                           OperationTargetLabel,
                                                                           OperationProtocolLabel,
                                                                           OperationTargetTypeLabel,
                                                                           OperationNameLabel,
                                                                           StatusLabel);
        }
    }
}