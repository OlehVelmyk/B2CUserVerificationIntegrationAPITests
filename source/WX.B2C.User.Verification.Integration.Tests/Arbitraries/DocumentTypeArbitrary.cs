using System.Linq;
using System.Reflection;
using FsCheck;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Integration.Tests.Arbitraries
{
    internal class DocumentTypeArbitrary : Arbitrary<DocumentType>
    {
        private static readonly DocumentType[] DocumentTypes;

        static DocumentTypeArbitrary()
        {
            var baseType = typeof(DocumentType);
            var assembly = baseType.Assembly;
            var documentTypes = assembly.GetTypes().Where(type => baseType.IsAssignableFrom(type));
            var properties = documentTypes.SelectMany(type => type.GetProperties(BindingFlags.Static | BindingFlags.Public));
            DocumentTypes = properties.Select(info => (DocumentType)info.GetMethod.Invoke(null, null)).ToArray();
        }

        public static Arbitrary<DocumentType> Create() => new DocumentTypeArbitrary();

        public override Gen<DocumentType> Generator => Gen.Elements(DocumentTypes);
    }
}
