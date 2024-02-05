using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services.Conditions
{
    internal class TinTypeConditionFactory : IConditionFactory
    {
        public ICondition Create(object config) =>
            new TinTypeCondition(config.ToString().To<TinType>());
    }
    internal class TinTypeCondition : ICondition
    {
        private readonly TinType _tinType;

        public TinTypeCondition(TinType tinType)
        {
            _tinType = tinType;
        }

        public static string[] RequiredData => new string[] { XPathes.Tin };

        public bool IsSatisfied(IDictionary<string, object> context)
        {
            var tin = context.Get<TinDto>(XPathes.Tin);
            return _tinType == tin.Type;
        }
    }
}