using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.PassFort.Models;
using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.PassFort
{
    internal class RiskScreeningCheckOutputDataExtractor : ICheckOutputDataExtractor
    {
        private readonly ICheckOutputDataSerializer _serializer;

        public RiskScreeningCheckOutputDataExtractor(ICheckOutputDataSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public IReadOnlyDictionary<string, object> Extract(string checkOutputData)
        {
            var data = _serializer.Deserialize<RiskScreeningCheckOutputData>(checkOutputData);
            return new Dictionary<string, object>
            {
                { nameof(VerificationProperty.IsPep), data.IsPep },
                { nameof(VerificationProperty.IsAdverseMedia), data.IsAdverseMedia },
                { nameof(VerificationProperty.IsSanctioned), data.IsSanctioned },
            };
        }
    }
}
