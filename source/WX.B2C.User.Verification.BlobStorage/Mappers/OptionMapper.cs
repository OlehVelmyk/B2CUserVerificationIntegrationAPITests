using System;
using System.Linq;
using WX.B2C.User.Verification.BlobStorage.Dto;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;

namespace WX.B2C.User.Verification.BlobStorage.Mappers
{
    internal interface IOptionMapper
    {
        CountriesOption Map(CountriesDto countries);

        PhoneCodesOption Map(PhoneCodeDto[] phoneCodes);

        ExcludedNamesOption Map(ExcludedNameDto[] excludedNames);
    }

    internal class OptionMapper : IOptionMapper
    {
        public CountriesOption Map(CountriesDto countries)
        {
            if (countries == null)
                throw new ArgumentNullException(nameof(countries));

            return new(countries.Countries.Select(Map));
        }

        public PhoneCodesOption Map(PhoneCodeDto[] phoneCodes)
        {
            if (phoneCodes == null)
                throw new ArgumentNullException(nameof(phoneCodes));

            return new(phoneCodes.Select(dto => new CountryPhoneCodeOption(dto.Alpha2Code, dto.PhoneCode)));
        }

        public ExcludedNamesOption Map(ExcludedNameDto[] excludedNames)
        {
            if (excludedNames == null)
                throw new ArgumentNullException(nameof(excludedNames));

            return new(excludedNames.Select(dto => new ExcludedNameOption(dto.FirstName, dto.LastName)));
        }

        private static CountryOption Map(CountryDto countryDto)
        {
            if (countryDto == null)
                throw new ArgumentNullException(nameof(countryDto));

            return new(countryDto.Alpha2Code,
                       countryDto.Alpha3Code,
                       countryDto.Name,
                       countryDto.States.Select(Map));
        }

        private static StateOption Map(StateDto stateDto)
        {
            if (stateDto == null)
                throw new ArgumentNullException(nameof(stateDto));

            return new(stateDto.Code, stateDto.Name);
        }
    }
}