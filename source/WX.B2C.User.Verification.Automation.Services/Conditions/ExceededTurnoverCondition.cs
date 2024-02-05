using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Automation.Services.Extensions;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services.Conditions
{
    internal class ExceededTurnoverConditionFactory : IConditionFactory
    {
        public ICondition Create(object config) =>
            new ExceededTurnoverCondition(config.To<ExceededTurnoverConditionConfig>());
    }

    internal class ExceededTurnoverCondition : ICondition
    {
        private readonly ExceededTurnoverConditionConfig _config;

        public ExceededTurnoverCondition(ExceededTurnoverConditionConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public static string[] RequiredData => new string[] { XPathes.Turnover, XPathes.RiskLevel };

        public bool IsSatisfied(IDictionary<string, object> context)
        {
            var riskLevel = context.Get<RiskLevel>(XPathes.RiskLevel);
            var turnover = context.Get<decimal>(XPathes.Turnover);
            return riskLevel == _config.RiskLevel && turnover >= _config.Threshold;
        }
    }

    internal class ExceededTurnoverConditionConfig
    {
        public RiskLevel RiskLevel { get; set; }

        public decimal Threshold { get; set; }
    }
}