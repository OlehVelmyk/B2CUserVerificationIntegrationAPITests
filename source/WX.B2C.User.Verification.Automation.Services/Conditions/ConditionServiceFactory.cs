using System;
using WX.B2C.User.Verification.Core.Contracts;

namespace WX.B2C.User.Verification.Automation.Services.Conditions
{
    internal class ConditionServiceFactory : IConditionServiceFactory, ITriggerConditionServiceFactory
    {
        private readonly IConditionsFactory _conditionsFactory;
        private readonly IProfileProviderFactory _dataProviderFactory;

        public ConditionServiceFactory(IConditionsFactory conditionsFactory, IProfileProviderFactory dataProviderFactory)
        {
            _conditionsFactory = conditionsFactory ?? throw new ArgumentNullException(nameof(conditionsFactory));
            _dataProviderFactory = dataProviderFactory ?? throw new ArgumentNullException(nameof(dataProviderFactory));
        }

        public IConditionService Create(Guid userId)
        {
            return new ConditionService(_conditionsFactory, _dataProviderFactory.Create(userId));
        }

        ITriggerConditionService ITriggerConditionServiceFactory.Create(Guid userId)
        {
            return new ConditionService(_conditionsFactory, _dataProviderFactory.Create(userId));
        }
    }
}