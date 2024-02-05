using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Conditions;
using WX.B2C.User.Verification.Core.Contracts.Triggers;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services.Conditions
{
    internal class ConditionService : IConditionService, ITriggerConditionService
    {
        private readonly IConditionsFactory _conditionsFactory;
        private readonly IProfileDataProvider _profileDataProvider;

        public ConditionService(IConditionsFactory conditionsFactory, IProfileDataProvider profileDataProvider)
        {
            _conditionsFactory = conditionsFactory ?? throw new ArgumentNullException(nameof(conditionsFactory));
            _profileDataProvider = profileDataProvider ?? throw new ArgumentNullException(nameof(profileDataProvider));
        }

        public string[] GetRequiredDataXPath(ConditionType conditionType) =>
            conditionType switch
            {
                ConditionType.RiskLevel         => RiskLevelCondition.RequiredData,
                ConditionType.TinType           => TinTypeCondition.RequiredData,
                ConditionType.IsPep             => IsPePCondition.RequiredData,
                ConditionType.MatchCountry      => MatchCountryCondition.RequiredData,
                ConditionType.ExceededTurnover  => ExceededTurnoverCondition.RequiredData,
                ConditionType.RepeatingTurnover => RepeatingTurnoverCondition.RequiredData,
                ConditionType.AccountAge        => AccountAgeCondition.RequiredData,
                ConditionType.MatchDecision     => MatchDecisionCondition.RequiredData,
                _                               => throw new ArgumentOutOfRangeException(nameof(conditionType), conditionType, null)
            };

        public Task<bool> IsAnySatisfiedAsync(Condition[] conditions)
        {
            var previousContextProperties = PreviousContextProperties(conditions);
            if (previousContextProperties.Any())
                throw new InvalidOperationException(
                $"Conditions requires previous context. Probably wrong conditions is used. Required properties:{string.Join(",", previousContextProperties)}");

            return IsAnySatisfiedAsync(conditions, new Dictionary<string, object>());
        }

        /// <summary>
        /// TODO Maybe better to move this method to <see cref="ITriggerService"/> as it can be used only there.
        /// TODO but in this case will be a lot of code duplication: reading data, reading required pathes and creating.
        /// TODO like a solution can be two implementation, two factories, and one base class with common functionality
        /// </summary>
        public async Task<bool> IsAnySatisfiedAsync(Condition[] conditions, IDictionary<string, object> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            foreach (var condition in conditions)
            {
                var isSatisfied = await IsSatisfiedAsync(condition, context);
                if (isSatisfied)
                    return true;
            }

            return false;
        }

        public string[] PreviousContextProperties(Condition[] conditions)
        {
            return conditions.Select(condition =>
            {
                return condition.Type switch
                {
                    ConditionType.RepeatingTurnover => RepeatingTurnoverCondition.PreviousContextData,
                    _                               => Enumerable.Empty<string>()
                };
            }).Flatten().Distinct().ToArray();
        }

        private async Task<bool> IsSatisfiedAsync(Condition conditionInfo, IDictionary<string, object> context)
        {
            var condition = _conditionsFactory.Create(conditionInfo);
            
            var xPathes = GetRequiredDataXPath(conditionInfo.Type);
            
            var requiredData = await _profileDataProvider.ReadAsync(xPathes);
            if (requiredData.Count != xPathes.Length)
                return false;

            requiredData.Foreach(pair => context[pair.Key] = pair.Value);

            return condition.IsSatisfied(context);
        }
    }
}