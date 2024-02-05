using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Conditions;

namespace WX.B2C.User.Verification.Core.Contracts
{
    public interface IConditionServiceFactory
    {
        IConditionService Create(Guid userId);
    }

    public interface IConditionService
    {
        string[] GetRequiredDataXPath(ConditionType conditionType);

        Task<bool> IsAnySatisfiedAsync(Condition[] conditions);
    }


    public interface ITriggerConditionServiceFactory
    {
        ITriggerConditionService Create(Guid userId);
    }

    public interface ITriggerConditionService
    {
        string[] PreviousContextProperties(Condition[] conditions);

        Task<bool> IsAnySatisfiedAsync(Condition[] conditions, IDictionary<string, object> context);
    }
}