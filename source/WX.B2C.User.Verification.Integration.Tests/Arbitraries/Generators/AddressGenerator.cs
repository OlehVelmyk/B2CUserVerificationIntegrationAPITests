using Bogus;
using FsCheck;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators
{
    internal class AddressGenerator
    {
        public static Gen<AddressDto> Address(int seed, string locale = "en")
        {
            var addressesDataSet = new Bogus.DataSets.Address
            {
                Random = new Randomizer(seed),
                Locale = locale
            };

            var address = new AddressDto
            {
                Country = addressesDataSet.CountryCode(),
                City = addressesDataSet.City(),
                State = addressesDataSet.StateAbbr(),
                ZipCode = addressesDataSet.ZipCode(),
                Line1 = addressesDataSet.StreetAddress(),
                Line2 = addressesDataSet.SecondaryAddress()
            };

            return Gen.Constant(address);
        }
    }
}
