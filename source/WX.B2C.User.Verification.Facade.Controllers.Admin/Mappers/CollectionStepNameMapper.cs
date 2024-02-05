using System;
using WX.B2C.User.Verification.Domain.Enums;
using WX.B2C.User.Verification.Domain.XPath;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    internal interface ICollectionStepNameMapper
    {
        string Map(XPathDetails xPath);
    }

    internal class CollectionStepNameMapper : ICollectionStepNameMapper
    {
        public string Map(XPathDetails xPath)
        {
            if (xPath == null)
                throw new ArgumentNullException(nameof(xPath));

            return xPath switch
            {
                DocumentsXPathDetails document => GetNameByCategory(document.Category),
                PropertyXPathDetails profile   => GetNameByProperty(profile.Property),
                SurveyXPathDetails             => "Survey",
                _                              => throw new ArgumentOutOfRangeException(nameof(xPath))
            };
        }

        private static string GetNameByCategory(DocumentCategory category)
        {
            return category switch
            {
                DocumentCategory.ProofOfIdentity => "Proof of identity",
                DocumentCategory.ProofOfAddress => "Proof of address",
                DocumentCategory.Supporting => "Supporting",
                DocumentCategory.Taxation => "Taxation",
                DocumentCategory.ProofOfFunds => "Proof Of funds",
                DocumentCategory.Selfie => "Selfie",
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            };
        }

        private static string GetNameByProperty(string property)
        {
            return property switch
            {
                nameof(VerificationProperty.IpAddress) => "Verification IP address",
                nameof(VerificationProperty.Tin) => "Taxpayer Identification Numbers",
                nameof(VerificationProperty.IdDocumentNumber) => "Proof of identity document number",
                nameof(VerificationProperty.TaxResidence) => "Tax residence countries",
                nameof(VerificationProperty.IsPep) => "Is politically exposed person",
                nameof(VerificationProperty.IsAdverseMedia) => "Is adverse media",
                nameof(VerificationProperty.IsSanctioned) => "Is sanctioned",
                nameof(PersonalProperty.FirstName) => "First name",
                nameof(PersonalProperty.LastName) => "Last name",
                nameof(PersonalProperty.FullName) => "Full name",
                nameof(PersonalProperty.Email) => "Email",
                nameof(PersonalProperty.Nationality) => "Nationality",
                nameof(PersonalProperty.ResidenceAddress) => "Address",
                nameof(PersonalProperty.Birthdate) => "Birthdate",
                _ => property
            };
        }
    }
}
