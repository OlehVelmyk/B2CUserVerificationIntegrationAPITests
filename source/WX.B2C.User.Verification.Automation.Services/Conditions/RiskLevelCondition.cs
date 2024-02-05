using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services.Conditions
{
    internal class RiskLevelConditionFactory : IConditionFactory
    {
        public ICondition Create(object config) =>
            new RiskLevelCondition(config.ToString().To<RiskLevel>());
    }

    internal class RiskLevelCondition : ICondition
    {
        private readonly RiskLevel _riskLevel;

        public RiskLevelCondition(RiskLevel riskLevel)
        {
            _riskLevel = riskLevel;
        }

        public static string[] RequiredData => new string[] { XPathes.RiskLevel };

        public bool IsSatisfied(IDictionary<string, object> context)
        {
            var riskLevel = context.Get<RiskLevel>(XPathes.RiskLevel);
            return _riskLevel == riskLevel;
        }
    }
}