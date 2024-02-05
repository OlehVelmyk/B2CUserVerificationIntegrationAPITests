using System;
using System.Linq;
using System.Collections.Generic;
using Serilog;
using Serilog.Events;

namespace WX.B2C.User.Verification.Unit.Tests
{
    internal class TestLogger : ILogger
    {
        private static readonly List<LogEvent> _logEvents = new();

        public void Write(LogEvent logEvent)
        {
            Console.WriteLine($"{logEvent.Level}: {logEvent.RenderMessage()}");
            _logEvents.Add(logEvent);
        }

        public bool HasErrors => _logEvents.Any(logEvent => logEvent.Exception != null);
        public bool HasAnyLog => _logEvents.Count > 0;
    }
}