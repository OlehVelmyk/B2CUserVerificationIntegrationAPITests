using System;
using System.Collections.Generic;
using System.Linq;

namespace WX.B2C.User.Verification.Core.Contracts.Configurations.Options
{
    public class PhoneCodesOption : Option
    {
        public PhoneCodesOption(IEnumerable<CountryPhoneCodeOption> countryPhoneCodes)
        {
            if (countryPhoneCodes == null)
                throw new ArgumentNullException(nameof(countryPhoneCodes));

            CountryPhoneCodes = countryPhoneCodes.ToDictionary(option => option.Country, option => option.PhoneCode);
        }

        public IReadOnlyDictionary<string, string> CountryPhoneCodes { get; }
    }

    public class CountryPhoneCodeOption
    {
        public CountryPhoneCodeOption(string country, string phoneCode)
        {
            Country = country ?? throw new ArgumentNullException(nameof(country));
            PhoneCode = phoneCode;
        }

        public string Country { get; }

        public string PhoneCode { get; }
    }
}
