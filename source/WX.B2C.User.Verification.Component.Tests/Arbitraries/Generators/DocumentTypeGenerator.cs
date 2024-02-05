using System.Linq;
using FsCheck;

namespace WX.B2C.User.Verification.Component.Tests.Arbitraries.Generators
{
    internal static class DocumentTypeGenerator
    {
        private static readonly string[] DocumentTypes =
        {
            "Passport",
            "IdentityCard",
            "DriverLicense",
            "BirthCertificate",
            "BankStatement",
            "UtilityBill",
            "TaxReturn",
            "CouncilTax",
            "CertificateOfResidency",
            "Other",
            "SourceOfFunds",
            "MyNumberCard",
            "ResidentRegisterCard",
            "ResidenceCard",
            "SpecialPermanentResidentCard",
            "MyNumberIdCard",
            "W9Form",
            "ResidencePermit",
            "PassportCard",
            "PostalIdentityCard",
            "Visa",
            "InternationalPassport",
            "SocialSecurityCard",
            "VoterId",
            "WorkPermit",
            "Video",
            "Photo"
        };

        public static Gen<string> Invalid() =>
            from type in StringGenerators.LettersOnly(4, 10)
            where !DocumentTypes.Contains(type)
            select type;

        public static Gen<string> Valid() =>
            from type in Gen.Elements(DocumentTypes)
            select type;
    }
}
