using System.Collections.Generic;

namespace WX.B2C.User.Verification.Automation.Services.Conditions
{
    internal interface ICondition
    {
        bool IsSatisfied(IDictionary<string, object> context);
    }
}