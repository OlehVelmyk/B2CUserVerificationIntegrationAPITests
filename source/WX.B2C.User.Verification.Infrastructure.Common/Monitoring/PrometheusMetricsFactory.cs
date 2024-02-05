using System;
using System.Collections.Generic;
using System.Linq;
using Prometheus;
using WX.B2C.User.Verification.Infrastructure.Common.Configuration;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Common.Monitoring
{
    public class PrometheusMetricsFactory
    {
        private static readonly QuantileEpsilonPair[] DefaultObjectives =
        {
            new(0.5, 0.05),
            new(0.9, 0.05),
            new(0.95, 0.01),
            new(0.99, 0.005)
        };
        private readonly IHostSettingsProvider _hostSettingsProvider;
        private readonly Dictionary<string, string> _staticLabels;

        public PrometheusMetricsFactory(IHostSettingsProvider hostSettingsProvider)
        {
            _hostSettingsProvider = hostSettingsProvider ?? throw new ArgumentNullException(nameof(hostSettingsProvider));

            _staticLabels = new Dictionary<string, string>();
            AddHostSettings(HostSettingsKey.ApplicationHost);
            AddHostSettings(HostSettingsKey.AppName);
            AddHostSettings(HostSettingsKey.NodeName);
            AddHostSettings(HostSettingsKey.Environment);
        }

        public Summary Summary(string name,
                               string description,
                               bool withObjectives,
                               params string[] labels) =>
            Metrics.CreateSummary(name,
                                  description,
                                  new SummaryConfiguration
                                  {
                                      StaticLabels = _staticLabels,
                                      Objectives = withObjectives ? DefaultObjectives : Array.Empty<QuantileEpsilonPair>(),
                                      LabelNames = labels?.Any() ?? false ? labels : null
                                  });

        public Histogram Histogram(string name, string description, params string[] labels) =>
            Metrics.CreateHistogram(name,
                                    description,
                                    new HistogramConfiguration
                                    {
                                        StaticLabels = _staticLabels,
                                        Buckets = Prometheus.Histogram.ExponentialBuckets(1, 2, 16),
                                        LabelNames = labels?.Any() ?? false ? labels : null
                                    });

        public Gauge Gauge(string name, string description, params string[] labels) =>
            Metrics.CreateGauge(name,
                                description,
                                new GaugeConfiguration
                                {
                                    StaticLabels = _staticLabels,
                                    LabelNames = labels?.Any() ?? false ? labels : null
                                });

        private void AddHostSettings(string key) =>
            _staticLabels.Add(key, _hostSettingsProvider.GetSetting(key));
    }
}