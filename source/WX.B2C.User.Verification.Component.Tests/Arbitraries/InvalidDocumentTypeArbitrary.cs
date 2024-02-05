using System;
using FsCheck;
using WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries
{
    internal class InvalidDocumentType
    {
        private readonly string _value;

        public InvalidDocumentType(string value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static implicit operator string(InvalidDocumentType documentType) {
            return documentType._value;
        }
    }

    internal class InvalidDocumentTypeArbitrary : Arbitrary<InvalidDocumentType>
    {
        public static Arbitrary<InvalidDocumentType> Create() => new InvalidDocumentTypeArbitrary();

        public override Gen<InvalidDocumentType> Generator =>
            from value in DocumentTypeGenerator.Invalid()
            select new InvalidDocumentType(value);
    }
}
