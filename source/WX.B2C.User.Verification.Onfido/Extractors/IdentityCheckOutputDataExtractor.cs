using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Onfido.Models;
using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.Onfido.Extractors
{
    internal class IdentityCheckOutputDataExtractor : ICheckOutputDataExtractor
    {
        private readonly ICheckOutputDataSerializer _serializer;

        public IdentityCheckOutputDataExtractor(ICheckOutputDataSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public IReadOnlyDictionary<string, object> Extract(string checkOutputData)
        {
            var data = _serializer.Deserialize<IdentityCheckOutputData>(checkOutputData);
            return new Dictionary<string, object>
            {
                { nameof(VerificationProperty.Nationality), data.Nationality },
                { nameof(VerificationProperty.PoiIssuingCountry), data.PoiIssuingCountry },
                { nameof(VerificationProperty.PlaceOfBirth), data.PlaceOfBirth },
                { nameof(VerificationProperty.IdDocumentNumber), data.IdDocumentNumber },
            };
        }
    }
}