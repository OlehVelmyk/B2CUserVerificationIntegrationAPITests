using System;
using Autofac.Features.Indexed;
using WX.B2C.User.Verification.Core.Contracts.Conditions;

namespace WX.B2C.User.Verification.Automation.Services.Conditions
{
    internal class ConditionsFactory : IConditionsFactory
    {
        private readonly IIndex<ConditionType, IConditionFactory> _factories;

        public ConditionsFactory(IIndex<ConditionType, IConditionFactory> factories)
        {
            _factories = factories ?? throw new ArgumentNullException(nameof(factories));
        }

        public ICondition Create(Condition conditionInfo)
        {
            if (!_factories.TryGetValue(conditionInfo.Type, out var factory))
                throw new ArgumentOutOfRangeException(nameof(conditionInfo.Type), conditionInfo.Type, "Factory for condition is not found");

            return factory.Create(conditionInfo.Value);
        }
    }
}