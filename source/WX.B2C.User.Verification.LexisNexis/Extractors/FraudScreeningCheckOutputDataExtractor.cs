using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.LexisNexis.Models;
using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.LexisNexis.Extractors
{
    internal class FraudScreeningCheckOutputDataExtractor : ICheckOutputDataExtractor
    {
        private readonly ICheckOutputDataSerializer _serializer;

        public FraudScreeningCheckOutputDataExtractor(ICheckOutputDataSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public IReadOnlyDictionary<string, object> Extract(string checkOutputData)
        {
            var data = _serializer.Deserialize<LexisNexisFraudScreeningOutputData>(checkOutputData);
            return new Dictionary<string, object>
            {
                { nameof(VerificationProperty.ComprehensiveIndex), data.ComprehensiveIndex },
                { nameof(data.RiskIndicators), data.RiskIndicators }
            };
        }
    }
}
