using System;
using System.Collections.Generic;
using Optional.Unsafe;
using WX.B2C.User.Verification.Automation.Services.Extensions;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services.Conditions
{
    internal class RepeatingTurnoverConditionFactory : IConditionFactory
    {
        public ICondition Create(object config) =>
            new RepeatingTurnoverCondition(config.To<RepeatingTurnoverConditionConfig>());
    }

    internal class RepeatingTurnoverCondition : ICondition
    {
        private const string ReachedRepeatingThreshold = nameof(ReachedRepeatingThreshold);
        private readonly RepeatingTurnoverConditionConfig _config;

        public RepeatingTurnoverCondition(RepeatingTurnoverConditionConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public static string[] RequiredData => new string[] { XPathes.Turnover, XPathes.RiskLevel };

        public static string[] PreviousContextData => new[] { ReachedRepeatingThreshold };

        public bool IsSatisfied(IDictionary<string, object> context)
        {
            var riskLevel = context.Get<RiskLevel>(XPathes.RiskLevel);
            if (riskLevel != _config.RiskLevel)
                return false;

            var turnover = context.Get<decimal>(XPathes.Turnover);
            //If some threshold already reached use it. Otherwise check if initial limit already reached.
            var reachedThreshold = context.Find<decimal>(ReachedRepeatingThreshold)
                                          .Or(_config.Threshold)
                                          .ValueOrFailure();
            var exceededSteps = Math.Floor((turnover - reachedThreshold) / _config.Step);
            if (exceededSteps < 1)
                return false;
                
            context[ReachedRepeatingThreshold] = exceededSteps * _config.Step + reachedThreshold;
            return true;
        }
    }

    internal class RepeatingTurnoverConditionConfig
    {
        public RiskLevel RiskLevel { get; set; }

        public decimal Threshold { get; set; }

        public decimal Step { get; set; }
    }
}