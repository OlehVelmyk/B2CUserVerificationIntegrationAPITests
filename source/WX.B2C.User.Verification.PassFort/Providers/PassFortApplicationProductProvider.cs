using System;
using System.Collections.Generic;
using System.Security;

namespace WX.B2C.User.Verification.PassFort.Providers
{
    public interface IPassFortApplicationProductProvider
    {
        SecureString FindProductId(string region, string country);
    }

    internal class PassFortApplicationProductProvider : IPassFortApplicationProductProvider
    {
        private readonly IReadOnlyDictionary<string, SecureString> _productIds;

        public PassFortApplicationProductProvider(
            IReadOnlyDictionary<string, SecureString> productIds)
        {
            _productIds = productIds ?? throw new ArgumentNullException(nameof(productIds));
        }

        public SecureString FindProductId(string region, string country)
        {
            if (region is null)
                throw new ArgumentNullException(nameof(region));
            if (country is null)
                throw new ArgumentNullException(nameof(country));

            var isSupported = _productIds.TryGetValue(country, out var result)
                              || _productIds.TryGetValue(region, out result)
                              || _productIds.TryGetValue("RoW", out result);

            return isSupported ? result : null;
        }
    }
}