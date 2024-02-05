using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services.Conditions
{
    internal class IsPePConditionFactory : IConditionFactory
    {
        public ICondition Create(object config) =>
            new IsPePCondition((bool)config);
    }

    internal class IsPePCondition : ICondition
    {
        private readonly bool _isPep;

        public IsPePCondition(bool isPep)
        {
            _isPep = isPep;
        }

        public static string[] RequiredData => new string[] { XPathes.IsPep };

        public bool IsSatisfied(IDictionary<string, object> context)
        {
            var isPep = ReadIsPep(context);
            return isPep == _isPep;
        }

        /// <summary>
        /// Ugly hack to allow reading from check output and from verification details at the same time.
        /// TODO WRXB-10220 https://wirexapp.atlassian.net/browse/WRXB-10220
        /// </summary>
        private static bool ReadIsPep(IDictionary<string, object> context)
        {
            var isPep = context.Find<bool>(VerificationProperty.IsPep);
            return isPep.ValueOr(() => context.Get<bool>(XPathes.IsPep));
        }
    }
}