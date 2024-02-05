using Bogus;
using FsCheck;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries
{
    internal class ResidenceAddressArbitrary : Arbitrary<AddressDto>
    {
        public static Arbitrary<AddressDto> Create() => new ResidenceAddressArbitrary();

        public override Gen<AddressDto> Generator =>
            from seed in Arb.Generate<int>()
            from countryCode in CountryCodeGenerators.Countries()
            from address in AddressGenerators.Address(seed, countryCode)
            select address;
    }
    
    internal static class AddressGenerators
    {
        public static Gen<AddressDto> Address(int seed, string country)
        {
            var addressesDataSet = new Bogus.DataSets.Address()
            {
                Random = new Randomizer(seed)
            };
        
            var address = new AddressDto
            {
                Country = country,
                City = addressesDataSet.City(),
                ZipCode = addressesDataSet.ZipCode(country == "GB" ? "??# #??" : null),
                Line1 = addressesDataSet.StreetAddress(),
                Line2 = addressesDataSet.SecondaryAddress()
            };

            return Gen.Constant(address);
        } 
    }

}
