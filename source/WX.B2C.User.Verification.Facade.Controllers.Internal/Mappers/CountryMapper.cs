using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Dtos;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Mappers
{
    public interface ICountryMapper
    {
        CountryDto Map(CountryOption country,
                       IReadOnlyCollection<RegionOption> regions,
                       IReadOnlyCollection<string> supportedCountries,
                       IReadOnlyDictionary<string, IReadOnlyCollection<string>> supportedStatesMap,
                       IReadOnlyDictionary<string, string> phoneCodesMap);
    }

    internal class CountryMapper : ICountryMapper
    {
        public CountryDto Map(CountryOption country,
                              IReadOnlyCollection<RegionOption> regions,
                              IReadOnlyCollection<string> supportedCountries,
                              IReadOnlyDictionary<string, IReadOnlyCollection<string>> supportedStatesMap,
                              IReadOnlyDictionary<string, string> phoneCodesMap)
        {
            if (country == null)
                throw new ArgumentNullException(nameof(country));
            if (regions == null)
                throw new ArgumentNullException(nameof(regions));
            if (supportedCountries == null)
                throw new ArgumentNullException(nameof(supportedCountries));
            if (supportedStatesMap == null)
                throw new ArgumentNullException(nameof(supportedStatesMap));
            if (phoneCodesMap == null)
                throw new ArgumentNullException(nameof(phoneCodesMap));

            var alpha2Code = country.Alpha2Code;
            var isSupported = supportedCountries.Contains(alpha2Code);
            var region = regions.FirstOrDefault(x => x.Countries.Contains(alpha2Code));
            var supportedStates = supportedStatesMap.GetValueOrDefault(alpha2Code);
            var isStateRequired = supportedStatesMap.ContainsKey(alpha2Code);
            var states = country.States.Select(MapState).ToArray();
            var phoneCode = phoneCodesMap.GetValueOrDefault(alpha2Code);

            return new CountryDto
            {
                Name = country.Name,
                Alpha2Code = alpha2Code,
                Alpha3Code = country.Alpha3Code,
                PhoneCode = phoneCode,
                Region = region?.Name,
                IsNotSupported = !isSupported,
                IsStateRequired = isStateRequired,
                States = states
            };

            StateDto MapState(StateOption state) => Map(state, supportedStates);
        }

        private static StateDto Map(StateOption state, IEnumerable<string> supportedStates)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            return new StateDto
            {
                Name = state.Name,
                Code = state.Code,
                IsSupported = supportedStates?.Contains(state.Code) ?? true
            };
        }
    }
}
