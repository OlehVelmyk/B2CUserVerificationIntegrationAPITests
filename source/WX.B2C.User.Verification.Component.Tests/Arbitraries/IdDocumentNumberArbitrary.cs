using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators;
using WX.B2C.User.Verification.Component.Tests.Constants;
using WX.B2C.User.Verification.Component.Tests.Models;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class IdDocumentNumberArbitrary : Arbitrary<IdDocumentNumber>
    {
        public static Arbitrary<IdDocumentNumber> Create() => new IdDocumentNumberArbitrary();

        public override Gen<IdDocumentNumber> Generator =>
            from number in StringGenerators.NotEmpty(6, 10)
            from documentType in Gen.Elements(DocumentTypes.IdentityDocumentTypes)
            select new IdDocumentNumber
            {
                Number = number,
                Type = documentType
            };
    }
}