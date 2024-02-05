using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Automation.Services.Extensions;
using WX.B2C.User.Verification.Core.Contracts.Extensions;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services.Conditions
{
    internal class AccountAgeConditionFactory : IConditionFactory
    {
        private readonly ISystemClock _systemClock;

        public AccountAgeConditionFactory(ISystemClock systemClock)
        {
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }

        public ICondition Create(object config) =>
            new AccountAgeCondition(config.To<AccountAgeConditionConfig>(), _systemClock);
    }

    internal class AccountAgeCondition : ICondition
    {
        private readonly AccountAgeConditionConfig _config;
        private readonly ISystemClock _systemClock;

        public AccountAgeCondition(AccountAgeConditionConfig config, ISystemClock systemClock)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }

        public static string[] RequiredData => new string[] { XPathes.RiskLevel, XPathes.ProfileCreationDate };

        public bool IsSatisfied(IDictionary<string, object> context)
        {
            var riskLevel = context.Get<RiskLevel>(XPathes.RiskLevel);
            if (riskLevel != _config.RiskLevel)
                return false;

            var creationDate = context.Get<DateTime>(XPathes.ProfileCreationDate);
            var overdueDate = creationDate.AddInterval(_config.Unit, _config.Limit);
            var today = _systemClock.GetDate();
            return today >= overdueDate;
        }
    }

    internal class AccountAgeConditionConfig
    {
        public RiskLevel RiskLevel { get; set; }

        public int Limit { get; set; }

        public IntervalUnit Unit { get; set; }
    }
}