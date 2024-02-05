using FsCheck;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.Generators;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries
{
    public class ResidenceAddressArbitrary : Arbitrary<AddressDto>
    {
        public static Arbitrary<AddressDto> Create()
        {
            return new ResidenceAddressArbitrary();
        }

        public override Gen<AddressDto> Generator =>
            from countryCode in CountryCodeGenerators.Countries()
            from stateCode in StringGenerators.LettersOnly(3,3)
            from city in StringGenerators.LettersOnly(2,20)
            from line1 in StringGenerators.NotEmpty(250)
            from line2 in StringGenerators.LettersOnly(1,250)
            from zipCode in StringGenerators.NotEmpty(50)
            select new AddressDto
            {
                Country = countryCode,
                State = stateCode,
                City = city,
                Line1 = line1,
                Line2 = line2,
                ZipCode = zipCode
            };
    }
}
