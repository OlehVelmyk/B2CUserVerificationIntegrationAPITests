using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;

namespace WX.B2C.User.Verification.Core.Services.Providers
{
    internal class CountryDetailsProvider : ICountryDetailsProvider
    {
        private readonly IOptionsProvider _optionsProvider;

        public CountryDetailsProvider(IOptionsProvider optionsProvider)
        {
            _optionsProvider = optionsProvider ?? throw new ArgumentNullException(nameof(optionsProvider));
        }

        public async Task<string> GetRegionAsync(string alpha2)
        {
            if (alpha2 == null)
                throw new ArgumentNullException(nameof(alpha2));

            alpha2 = NormalizeString(alpha2);
            var option = await _optionsProvider.GetAsync<RegionsOption>();
            var region = option.Regions.FirstOrDefault(option => option.Countries.Contains(alpha2));
            if (region == null)
                throw new ArgumentException($"Cannot find region for country with alpha2 :{alpha2}", nameof(alpha2));

            return region.Name;
        }

        public async Task<bool> IsSupportedAsync(string alpha2)
        {
            if (alpha2 == null)
                throw new ArgumentNullException(nameof(alpha2));

            alpha2 = NormalizeString(alpha2);
            var option = await _optionsProvider.GetAsync<SupportedCountriesOption>();
            return option.Countries.Contains(alpha2);
        }

        public async Task<string> GetAlpha3Async(string alpha2)
        {
            var alpha3 = await FindAlpha3Async(alpha2);
            return alpha3 ?? throw new InvalidOperationException($"Cannot find country option for {alpha2}");
        }

        public async Task<string> FindAlpha3Async(string alpha2)
        {
            if (alpha2 == null)
                throw new ArgumentNullException(nameof(alpha2));

            alpha2 = NormalizeString(alpha2);
            var option = await _optionsProvider.GetAsync<CountriesOption>();
            var countryOption = option.Countries.FirstOrDefault(o => o.Alpha2Code == alpha2);

            return countryOption?.Alpha3Code;
        }

        public async Task<string> FindAlpha2Async(string alpha3)
        {
            if (alpha3 == null)
                throw new ArgumentNullException(nameof(alpha3));

            alpha3 = NormalizeString(alpha3);
            var countriesOption = await _optionsProvider.GetAsync<CountriesOption>();
            var countryOption = countriesOption.Countries.FirstOrDefault(o => o.Alpha3Code == alpha3);

            var aplpha2Code = countryOption?.Alpha2Code;
            if (aplpha2Code == null)
            {
                var specificCountriesOtion = await _optionsProvider.GetAsync<SpecificCountriesOption>();
                var specificOption = specificCountriesOtion.Countries.FirstOrDefault(o => o.IcaoCodes.Contains(alpha3));
                aplpha2Code = specificOption?.Alpha2Code;
            }

            return aplpha2Code;
        }

        private static string NormalizeString(string s) => s.ToUpperInvariant();
    }
}