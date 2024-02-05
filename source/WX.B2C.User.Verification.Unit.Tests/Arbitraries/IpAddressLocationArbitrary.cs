using FsCheck;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Unit.Tests.Arbitraries.Generators;

namespace WX.B2C.User.Verification.Unit.Tests.Arbitraries
{
    internal class IpAddressLocationArbitrary : Arbitrary<IpAddressLocation>
    {
        public static Arbitrary<IpAddressLocation> Create()
        {
            return new IpAddressLocationArbitrary();
        }

        public override Gen<IpAddressLocation> Generator =>
            from countryCode in CountryCodeGenerators.Countries()
            from countryName in Arb.Generate<string>()
            from stateCode in Arb.Generate<string>()
            from stateName in Arb.Generate<string>()
            from city in Arb.Generate<string>()
            from continentCode in Arb.Generate<string>()
            from continentName in Arb.Generate<string>()
            from line2 in Arb.Generate<string>()
            from zipCode in Arb.Generate<string>()
            select new IpAddressLocation
            {
                CountryCode = countryCode,
                CountryName = countryName,
                StateCode = stateCode,
                StateName = stateName,
                City = city,
                ContinentCode = continentCode,
                ContinentName = continentName
            };
    }
}
