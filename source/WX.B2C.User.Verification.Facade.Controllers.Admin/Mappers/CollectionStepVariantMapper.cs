using System;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Enums;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    internal interface ICollectionStepVariantMapper
    {
        CollectionStepVariantDto Map(XPathDetails xPath);
    }

    internal class CollectionStepVariantMapper : ICollectionStepVariantMapper
    {
        private readonly ICollectionStepNameMapper _stepNameMapper;

        public CollectionStepVariantMapper(ICollectionStepNameMapper stepNameMapper)
        {
            _stepNameMapper = stepNameMapper ?? throw new ArgumentNullException(nameof(stepNameMapper));
        }

        public CollectionStepVariantDto Map(XPathDetails xPath)
        {
            if (xPath == null)
                throw new ArgumentNullException(nameof(xPath));

            CollectionStepVariantDto variant = xPath switch
            {
                PropertyXPathDetails { Source: PropertySource.Personal } xPathDetails => PersonalDetailsCollectionStep(xPathDetails),
                PropertyXPathDetails { Source: PropertySource.Verification } xPathDetails => VerificationDetailsCollectionStep(xPathDetails),
                DocumentsXPathDetails documentXPath => DocumentCollectionStep(documentXPath),
                SurveyXPathDetails surveyXPath => SurveyCollectionStep(surveyXPath),
                _ => throw new ArgumentOutOfRangeException(nameof(xPath), xPath.GetType(), "Unsupported xPath type.")
            };

            variant.Name = _stepNameMapper.Map(xPath);
            return variant;
        }

        private static PersonalDetailsCollectionStepVariantDto PersonalDetailsCollectionStep(PropertyXPathDetails xPathDetails)
        {
            return new PersonalDetailsCollectionStepVariantDto
            {
                Property = MapPersonalDetailsProperty(xPathDetails.Property)
            };
        }

        private static VerificationDetailsCollectionStepVariantDto VerificationDetailsCollectionStep(PropertyXPathDetails xPathDetails)
        {
            return new VerificationDetailsCollectionStepVariantDto
            {
                Property = MapVerificationDetailsProperty(xPathDetails.Property)
            };
        }

        private static DocumentCollectionStepVariantDto DocumentCollectionStep(DocumentsXPathDetails xPathDetails)
        {
            return new DocumentCollectionStepVariantDto
            {
                DocumentCategory = xPathDetails.Category,
                DocumentType = xPathDetails.Type
            };
        }

        private static SurveyCollectionStepVariantDto SurveyCollectionStep(SurveyXPathDetails xPathDetails)
        {
            return new SurveyCollectionStepVariantDto
            {
                TemplateId = xPathDetails.SurveyId
            };
        }

        private static PersonalDetailsProperty MapPersonalDetailsProperty(string property)
        {
            return property switch
            {
                PersonalProperty.FirstName => PersonalDetailsProperty.FirstName,
                PersonalProperty.LastName => PersonalDetailsProperty.LastName,
                PersonalProperty.Birthdate => PersonalDetailsProperty.Birthdate,
                PersonalProperty.ResidenceAddress => PersonalDetailsProperty.ResidenceAddress,
                PersonalProperty.Nationality => PersonalDetailsProperty.Nationality,
                PersonalProperty.Email => PersonalDetailsProperty.Email,
                PersonalProperty.FullName => PersonalDetailsProperty.FullName,
                _ => throw new ArgumentOutOfRangeException(nameof(property), property, "Unsupported property type.")
            };
        }

        private static VerificationDetailsProperty MapVerificationDetailsProperty(string property)
        {
            return property switch
            {
                VerificationProperty.IpAddress => VerificationDetailsProperty.IpAddress,
                VerificationProperty.TaxResidence => VerificationDetailsProperty.TaxResidence,
                VerificationProperty.RiskLevel => VerificationDetailsProperty.RiskLevel,
                VerificationProperty.IdDocumentNumber => VerificationDetailsProperty.IdDocumentNumber,
                VerificationProperty.Tin => VerificationDetailsProperty.Tin,
                VerificationProperty.Nationality => VerificationDetailsProperty.Nationality,
                VerificationProperty.IsPep => VerificationDetailsProperty.IsPep,
                VerificationProperty.IsSanctioned => VerificationDetailsProperty.IsSanctioned,
                VerificationProperty.IsAdverseMedia => VerificationDetailsProperty.IsAdverseMedia,
                VerificationProperty.Turnover => VerificationDetailsProperty.Turnover,
                VerificationProperty.PoiIssuingCountry => VerificationDetailsProperty.PoiIssuingCountry,
                VerificationProperty.PlaceOfBirth => VerificationDetailsProperty.PlaceOfBirth,
                VerificationProperty.ComprehensiveIndex => VerificationDetailsProperty.ComprehensiveIndex,
                VerificationProperty.IsIpMatched => VerificationDetailsProperty.IsIpMatched,
                VerificationProperty.ResolvedCountryCode => VerificationDetailsProperty.ResolvedCountryCode,
                _ => throw new ArgumentOutOfRangeException(nameof(property), property, "Unsupported property type.")
            };
        }
    }
}