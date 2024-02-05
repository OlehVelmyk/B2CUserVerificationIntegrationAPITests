using WX.B2C.User.Verification.Core.Contracts.Conditions;

namespace WX.B2C.User.Verification.Automation.Services.Conditions
{
    internal interface IConditionsFactory
    {
        ICondition Create(Condition config);
    }
}