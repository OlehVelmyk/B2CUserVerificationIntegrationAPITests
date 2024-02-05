using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Provider.Contracts;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    internal class IpAddressOutputDataExtractor : ICheckOutputDataExtractor
    {
        private readonly ICheckOutputDataSerializer _serializer;

        public IpAddressOutputDataExtractor(ICheckOutputDataSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        // Now extract only country code from location, maybe need to extract all location
        public IReadOnlyDictionary<string, object> Extract(string checkOutputData)
        {
            var data = _serializer.Deserialize<IpAddressCheckOutputData>(checkOutputData);
            return new Dictionary<string, object>
            {
                { nameof(VerificationProperty.IsIpMatched), data.IsIpMatched },
                { nameof(VerificationProperty.ResolvedCountryCode), data.ResolvedLocation?.CountryCode },
            };
        }
    }
}
