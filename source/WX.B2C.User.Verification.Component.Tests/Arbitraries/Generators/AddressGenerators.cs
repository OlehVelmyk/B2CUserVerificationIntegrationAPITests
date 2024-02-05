using Bogus;
using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Extensions;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators
{
    internal static class AddressGenerators
    {
        public static Gen<Address> Address(Seed seed, string country)
        {
            var addressesDataSet = new Bogus.DataSets.Address()
            {
                Random = new Randomizer(seed)
            };
        
            var address = new Address
            {
                Country = country,
                City = addressesDataSet.City(),
                ZipCode = addressesDataSet.ZipCode(country == CountryCodes.Gb ? "??# #??" : null),
                Line1 = addressesDataSet.StreetAddress(),
                Line2 = addressesDataSet.SecondaryAddress()
            };
            
            return Gen.Constant(address).Override(
                StateGenerators.Us(), 
                a => a.State,
                a => a.Country == CountryCodes.Us);
        } 
    }
}