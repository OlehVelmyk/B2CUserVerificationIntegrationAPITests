using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class AddressArbitrary : Arbitrary<Address>
    {
        public static Arbitrary<Address> Create() => new AddressArbitrary();

        public override Gen<Address> Generator =>
            from seed in Arb.Generate<Seed>()
            from countryCode in CountryCodeGenerators.Supported()
            from address in AddressGenerators.Address(seed, countryCode)
            select address;
    }

    internal class InvalidAddressArbitrary : Arbitrary<InvalidAddress>
    {
        public static Arbitrary<InvalidAddress> Create() => new InvalidAddressArbitrary();

        public override Gen<InvalidAddress> Generator =>
            from country in Arb.Generate<string>()
            where country?.Length != 2
            from city in Arb.Generate<string>().OrNull()
            from state in Arb.Generate<string>().OrNull()
            from zipCode in Arb.Generate<string>().OrNull()
            from line1 in Arb.Generate<string>().OrNull()
            from line2 in Arb.Generate<string>().OrNull()
            select new InvalidAddress
            {
                Country = country,
                City = city,
                State = state,
                ZipCode = zipCode,
                Line1 = line1,
                Line2 = line2
            };
    }
}
