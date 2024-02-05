using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Dtos
{
    /// <summary>
    /// TODO to have clear swagger json we need some discriminator by action type. Potentially we can use subtypes 
    /// </summary>
    public class UserActionDto
    {
        public ActionType ActionType { get; set; }

        public string Reason { get; set; }

        public bool IsOptional { get; set; }

        /// <summary>
        /// TODO always zero for the first iteration
        /// </summary>
        public int Priority { get; set; }

        public Dictionary<string, string> ActionData { get; set; }
    }
}