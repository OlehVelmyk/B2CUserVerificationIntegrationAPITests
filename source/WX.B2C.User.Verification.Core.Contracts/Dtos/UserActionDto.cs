using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class UserActionDto
    {
        public ActionType ActionType { get; set; }

        public string Reason { get; set; }

        public int Priority { get; set; }

        public bool IsOptional { get; set; }

        public Dictionary<string, string> ActionData { get; set; }
    }
}