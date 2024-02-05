using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace WX.B2C.User.Verification.Infrastructure.Common.Logging
{
    internal static class LoggerFilterConfigurationExtensions
    {
        public static LoggerConfiguration UniqueOverTimeSpan(this LoggerFilterConfiguration configuration, Func<LogEvent, bool> predicate, TimeSpan span)
        {
            return
                configuration
                    .With(new UniqueOverSpanFilter(predicate, span));
        }

        public static bool HasProperty(this LogEvent @event, string propertyName, string propertyValue)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            return @event.Properties.TryGetValue(propertyName, out var property)
                   && string.Equals(property.ToString().Replace("\"", ""), propertyValue);
        }
    }

    internal class UniqueOverSpanFilter : ILogEventFilter
    {
        private readonly Func<LogEvent, bool> _isEnabled;
        private readonly TimeSpan _timeSpan;
        private DateTimeOffset _lastEventTimestamp;
        private int _eventsCounter;

        public UniqueOverSpanFilter(Func<LogEvent, bool> isEnabled, TimeSpan timeSpan)
        {
            _isEnabled = isEnabled ?? throw new ArgumentNullException(nameof(isEnabled));
            _timeSpan = timeSpan;
        }

        public bool IsEnabled(LogEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            if (_isEnabled(@event))
            {
                var eventTimestamp = @event.Timestamp;
                if (eventTimestamp <= _lastEventTimestamp.Add(_timeSpan))
                {
                    _eventsCounter++;
                    return false;
                }

                @event.AddOrUpdateProperty(new LogEventProperty("SkippedLogEvents", new ScalarValue(_eventsCounter)));
                @event.AddOrUpdateProperty(new LogEventProperty("UniqueOverSpan", new ScalarValue(_timeSpan)));
                @event.AddOrUpdateProperty(new LogEventProperty("LastEventTimestamp", new ScalarValue(_lastEventTimestamp)));

                _lastEventTimestamp = eventTimestamp;
                _eventsCounter = 0;
            }

            return true;
        }
    }
}
