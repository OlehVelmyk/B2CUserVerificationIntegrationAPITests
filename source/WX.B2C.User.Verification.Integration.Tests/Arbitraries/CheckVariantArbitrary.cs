using System;
using System.Collections.Generic;
using FsCheck;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Utilities;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries
{
    internal class CheckVariantArbitrary : Arbitrary<CheckVariantSpecimen>
    {
        public static Arbitrary<CheckVariantSpecimen> Create() => new CheckVariantArbitrary();

        public override Gen<CheckVariantSpecimen> Generator =>
            from id in Arb.Generate<Guid>()
            from provider in Arb.Generate<CheckProviderType>()
            from type in Arb.Generate<CheckType>()
            from maxAttempts in Arb.Generate<int?>().OrNull()
            select new CheckVariantSpecimen
            {
                Id = id,
                Type = type,
                Provider = provider,
                MaxAttempts = maxAttempts
            };
    }
}
