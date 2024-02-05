using Serilog.Events;

namespace WX.B2C.User.Verification.Configuration.Models
{
    public class HostLogLevel 
    {
        public string Host { get; set; }

        public LogEventLevel Level { get; set; }
    }
}