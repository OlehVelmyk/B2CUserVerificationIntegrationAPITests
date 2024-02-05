using System;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    internal interface ICollectionStepDataMapper
    {
        object Map(XPathDetails xPath, object collectedData);
    }

    internal class CollectionStepDataMapper : ICollectionStepDataMapper
    {
        private readonly IVerificationDetailsMapper _verificationDetailsMapper;
        private readonly IDocumentMapper _documentMapper;

        public CollectionStepDataMapper(
            IVerificationDetailsMapper verificationDetailsMapper,
            IDocumentMapper documentMapper)
        {
            _verificationDetailsMapper = verificationDetailsMapper ?? throw new ArgumentNullException(nameof(verificationDetailsMapper));
            _documentMapper = documentMapper ?? throw new ArgumentNullException(nameof(documentMapper));
        }

        public object Map(XPathDetails xPath, object collectedData)
        {
            if (xPath == null)
                throw new ArgumentNullException(nameof(xPath));

            return xPath switch
            {
                PropertyXPathDetails { Source: PropertySource.Personal } xPathDetails => MapPersonalDetailsData(xPathDetails.Property, collectedData),
                PropertyXPathDetails { Source: PropertySource.Verification } xPathDetails => MapVerificationDetailsData(xPathDetails.Property, collectedData),
                DocumentsXPathDetails _ => MapDocumentData(collectedData),
                SurveyXPathDetails _ => MapSurveyData(),
                _ => throw new ArgumentOutOfRangeException(nameof(xPath), xPath, "Unsupported xPath.")
            };
        }

        private object MapPersonalDetailsData(string propertyName, object value)
        {
            return value;
        }

        private object MapVerificationDetailsData(string propertyName, object value)
        {
            return propertyName switch
            {
                VerificationProperty.IpAddress => (string)value,
                VerificationProperty.Tin => _verificationDetailsMapper.SafeMap((Core.Contracts.Dtos.Profile.TinDto)value),
                VerificationProperty.IdDocumentNumber => _verificationDetailsMapper.SafeMap((Core.Contracts.Dtos.Profile.IdDocumentNumberDto)value),
                VerificationProperty.TaxResidence => (string[])value,
                VerificationProperty.Nationality => (string)value,
                VerificationProperty.IsPep => (bool?)value,
                VerificationProperty.IsSanctioned => (bool?)value,
                VerificationProperty.IsAdverseMedia => (bool?)value,
                VerificationProperty.ComprehensiveIndex => (int)value,
                VerificationProperty.PlaceOfBirth => (string)value,
                VerificationProperty.PoiIssuingCountry => (string)value,
                VerificationProperty.ResolvedCountryCode => (string)value,
                VerificationProperty.RiskLevel => (string)value,
                VerificationProperty.Turnover => (decimal)value,
                VerificationProperty.IsIpMatched => (bool?)value,
                _ => throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, "Unsupported property type.")
            };
        }

        private DocumentDto MapDocumentData(object value)
        {
            var documentDto = (Core.Contracts.Dtos.DocumentDto)value;
            return value != null ? _documentMapper.Map(documentDto) : null;
        }

        private static object MapSurveyData()
        {
            // TODO: Investigate which data should be returned for survey
            return default;
        }
    }
}