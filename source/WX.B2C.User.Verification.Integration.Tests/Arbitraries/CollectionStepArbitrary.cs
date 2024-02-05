using System;
using FsCheck;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Integration.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries
{
    internal class CollectionStepArbitrary : Arbitrary<CollectionStepSpecimen>
    {
        public static Arbitrary<CollectionStepSpecimen> Create() => new CollectionStepArbitrary();

        public override Gen<CollectionStepSpecimen> Generator =>
            from id in Arb.Generate<Guid>()
            from userId in Arb.Generate<Guid>()
            from xPath in StringGenerators.NotEmpty(10)
            from state in Arb.Generate<CollectionStepState>()
            from isRequired in Arb.Generate<bool>()
            select new CollectionStepSpecimen
            {
                Id = id,
                UserId = userId,
                XPath = xPath,
                State = state,
                IsRequired = isRequired
            };
    }
}
