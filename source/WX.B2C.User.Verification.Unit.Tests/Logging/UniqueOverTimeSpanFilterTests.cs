using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using WX.B2C.User.Verification.Infrastructure.Common.Logging;

namespace WX.B2C.User.Verification.Unit.Tests.Logging
{
    internal class TestSink : ILogEventSink
    {
        public List<LogEvent> Events { get; } = new();

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage();
            Console.WriteLine(DateTimeOffset.Now + " " + message);

            Events.Add(logEvent);
        }
    }

    internal static class LogEventExtensions
    {
        public static T GetScalarValue<T>(this LogEvent @event, string name)
        {
            if (@event.Properties[name] is ScalarValue scalarValue) return (T)scalarValue.Value;

            throw new InvalidOperationException("Requested property is not scalar value.");
        }
    }

    [TestFixture]
    internal class UniqueOverTimeSpanFilterTests
    {
        private TestSink _testSink;
        private LoggerConfiguration _loggerConfiguration;

        [SetUp]
        public void Setup()
        {
            _testSink = new TestSink();
            _loggerConfiguration = new LoggerConfiguration()
                                   .Enrich.WithProperty("Source", "Test")
                                   .WriteTo.Sink(_testSink)
                                   .Filter.UniqueOverTimeSpan(@event => 
                                       @event.Level == LogEventLevel.Information && 
                                       @event.HasProperty("Source", "Test"), TimeSpan.FromSeconds(5));
        }

        [TearDown]
        public void TearDown()
        {
            _testSink.Events.Clear();
        }

        [Test]
        public void ShouldLogOnlyOneEventOverTimeSpan()
        {
            var logger = _loggerConfiguration.CreateLogger();

            foreach (var number in Enumerable.Range(1, 10))
            {
                logger.Information("Test sample number: {Number}", number);
                Thread.Sleep(500);
            }
            _testSink.Events.Should().HaveCount(1);
            _testSink.Events.Should().OnlyContain(@event => @event.GetScalarValue<int>("Number") == 1);
        }

        [Test]
        public void ShouldLogOnlyOneEventPerEachTimeSpan()
        {
            var logger = _loggerConfiguration.CreateLogger();

            foreach (var number in Enumerable.Range(1, 50))
            {
                logger.Information("Test sample number: {Number}", number);
                Thread.Sleep(200);
            }
            _testSink.Events.Should().HaveCount(2);
            var (event1, event2) = (_testSink.Events[0], _testSink.Events[1]);
            event1.GetScalarValue<int>("Number").Should().Be(1);
            event2.GetScalarValue<int>("Number").Should().Be(26);
            event2.GetScalarValue<int>("SkippedLogEvents").Should().Be(24);
            event2.GetScalarValue<DateTimeOffset>("LastEventTimestamp").Should().Be(event1.Timestamp);
        }

        [Test]
        public void ShouldLogEvent_WhenPredicateNotSatisfied()
        {
            var logger = _loggerConfiguration.CreateLogger();

            foreach (var number in Enumerable.Range(1, 10))
            {
                logger.Warning("Test sample number: {Number}", number);
                Thread.Sleep(500);
            }
            _testSink.Events.Should().HaveCount(10);
        }


        [Test]
        public void ShouldNotAffectOtherEvents()
        {
            var logger = _loggerConfiguration.CreateLogger();

            foreach (var number in Enumerable.Range(1, 10))
            {
                if (number % 2 != 0)
                    logger.Information("Test sample number: {Number}", number);
                else
                    logger.Warning("Test sample number: {Number}", number);
            }

            _testSink.Events.Should().HaveCount(6);
            _testSink.Events.Should().ContainSingle(@event => @event.Level == LogEventLevel.Information);

        }
    }
}
