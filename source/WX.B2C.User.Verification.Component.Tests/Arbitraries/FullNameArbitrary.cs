using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class FullNameArbitrary : Arbitrary<FullName>
    {
        public static Arbitrary<FullName> Create() => new FullNameArbitrary();

        public override Gen<FullName> Generator =>
            from seed in Arb.Generate<Seed>()
            let faker = FakerFactory.Create(seed)
            select new FullName
            {
                FirstName = faker.Name.FirstName(),
                LastName = faker.Name.LastName(),
            };
    }
}
