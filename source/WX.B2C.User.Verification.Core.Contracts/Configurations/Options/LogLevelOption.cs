using System.Collections.Generic;
using Serilog.Events;

namespace WX.B2C.User.Verification.Core.Contracts.Configurations.Options
{
    public class LogLevelOption : Option
    {
        private readonly Dictionary<string, LogEventLevel> _hostsLogLevels;

        public LogLevelOption(Dictionary<string, LogEventLevel> hostsLogLevels = null)
        {
            _hostsLogLevels = hostsLogLevels ?? new Dictionary<string, LogEventLevel>();
        }

        public LogEventLevel Get(string hostName, LogEventLevel defaultLevel) =>
            _hostsLogLevels.TryGetValue(hostName, out var specific) ? specific : defaultLevel;
    }
}