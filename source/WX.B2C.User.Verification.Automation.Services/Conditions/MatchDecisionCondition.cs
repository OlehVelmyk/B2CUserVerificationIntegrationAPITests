using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services.Conditions
{
    internal class MatchDecisionConditionFactory : IConditionFactory
    {
        public ICondition Create(object config) =>
            new MatchDecisionCondition((string)config);
    }

    internal class MatchDecisionCondition : ICondition
    {
        public const string DecisionRequiredData = "Decision";

        private readonly string _expectedDecision;

        public MatchDecisionCondition(string expectedDecision)
        {
            _expectedDecision = expectedDecision ?? throw new ArgumentNullException(nameof(expectedDecision));
        }

        public static string[] RequiredData => new[] { DecisionRequiredData };

        public bool IsSatisfied(IDictionary<string, object> context)
        {
            var decision = context.Get<string>(DecisionRequiredData);
            return string.Equals(decision, _expectedDecision);
        }
    }
}