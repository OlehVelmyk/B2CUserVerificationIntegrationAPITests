using System.Collections.Generic;

namespace WX.B2C.User.Verification.Configuration.Models
{
    public class Ticket
    {
        public string Reason { get; set; }

        public string[] Parameters { get; set; }
        
        public Dictionary<string,string> Formats { get; set; }
    }
    
    public class ParametersMapping
    {
        public string Name { get; set; }

        public string Source { get; set; }
    }
}