using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal class TaxResidenceOutputDataExtractor : ICheckOutputDataExtractor
    {
        private readonly ICheckOutputDataSerializer _serializer;

        public TaxResidenceOutputDataExtractor(ICheckOutputDataSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public IReadOnlyDictionary<string, object> Extract(string checkOutputData)
        {
            var data = _serializer.Deserialize<TaxResidenceOutputData>(checkOutputData);
            return new Dictionary<string, object>
            {
                { nameof(data.MatchedCountries), data.MatchedCountries },
            };
        }
    }
}