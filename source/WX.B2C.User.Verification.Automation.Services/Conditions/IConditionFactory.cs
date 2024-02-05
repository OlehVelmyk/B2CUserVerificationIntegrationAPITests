namespace WX.B2C.User.Verification.Automation.Services.Conditions
{
    internal interface IConditionFactory
    {
        ICondition Create(object config);
    }
}