using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Configuration.Models
{
    public class RegionActions
    {
        public RegionType RegionType { get; set; }

        public string Region { get; set; }

        public Action[] Actions { get; set; }
    }

    public class Action
    {
        public ActionType ActionType { get; set; }

        public string XPath { get; set; }

        public int Priority { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}