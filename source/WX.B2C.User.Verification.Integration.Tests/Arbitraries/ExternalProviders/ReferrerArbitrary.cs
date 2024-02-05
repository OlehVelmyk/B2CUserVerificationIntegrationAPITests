using FsCheck;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries.ExternalProviders
{
    internal class ReferrerArbitrary : Arbitrary<Referrer>
    {
        private const int MaxLength = 10;

        public static Arbitrary<Referrer> Create() => new ReferrerArbitrary();

        public override Gen<Referrer> Generator => 
            from referrer in ReferrerGenerator.Referrer(MaxLength)
            select new Referrer
            {
                Value = referrer
            };
    }

    internal class InvalidReferrerArbitrary : Arbitrary<InvalidReferrer>
    {
        private const int MaxLength = 10;

        public static Arbitrary<InvalidReferrer> Create() => new InvalidReferrerArbitrary();

        public override Gen<InvalidReferrer> Generator =>
            from referrer in ReferrerGenerator.InvalidReferrer(MaxLength)
            select new InvalidReferrer
            {
                Value = referrer
            };
    }
}
