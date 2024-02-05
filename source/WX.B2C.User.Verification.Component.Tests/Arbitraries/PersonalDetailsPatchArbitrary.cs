using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class PersonalDetailsPatchArbitrary : Arbitrary<PersonalDetailsPatch>
    {
        public static Arbitrary<PersonalDetailsPatch> Create() => new PersonalDetailsPatchArbitrary();

        public override Gen<PersonalDetailsPatch> Generator =>
            from properties in PersonalDetailsPropertyGenerator.Generate()
            select new PersonalDetailsPatch
            {
                Properties = properties
            };
    }
}