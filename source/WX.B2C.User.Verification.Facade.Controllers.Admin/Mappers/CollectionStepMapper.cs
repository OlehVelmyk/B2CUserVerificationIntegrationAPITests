using System;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Enums;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using CoreDtos = WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface ICollectionStepMapper
    {
        CoreDtos.NewCollectionStepDto Map(CollectionStepRequest request);

        CollectionStepDto Map(CoreDtos.CollectionStepDto stepDto, object collectedData, Guid[] relatedTasks);

        CoreDtos.CollectionStepPatch Map(UpdateCollectionStepRequest request);
    }

    internal class CollectionStepMapper : ICollectionStepMapper
    {
        private readonly ICollectionStepVariantMapper _stepVariantMapper;
        private readonly ICollectionStepDataMapper _stepDataMapper;
        private readonly IXPathParser _xPathParser;

        public CollectionStepMapper(
            ICollectionStepVariantMapper stepVariantMapper,
            ICollectionStepDataMapper stepDataMapper,
            IXPathParser xPathParser)
        {
            _stepVariantMapper = stepVariantMapper ?? throw new ArgumentNullException(nameof(stepVariantMapper));
            _stepDataMapper = stepDataMapper ?? throw new ArgumentNullException(nameof(stepDataMapper));
            _xPathParser = xPathParser ?? throw new ArgumentNullException(nameof(xPathParser));
        }

        public CoreDtos.NewCollectionStepDto Map(CollectionStepRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var stepType = request.Type;
            var xPath = stepType switch
            {
                CollectionStepType.VerificationDetails => BuildVerificationDetailsXpath((VerificationDetailsCollectionStepRequest)request),
                CollectionStepType.PersonalDetails => BuildPersonalDetailsXpath((PersonalDetailsCollectionStepRequest)request),
                CollectionStepType.Document => BuildDocumentXPath((DocumentCollectionStepRequest)request),
                CollectionStepType.Survey => BuildSurveyXPath((SurveyCollectionStepRequest)request),
                _ => throw new ArgumentOutOfRangeException(nameof(stepType), stepType, "Unsupported collection step type.")
            };

            return new CoreDtos.NewCollectionStepDto
            {
                XPath = xPath,
                IsRequired = request.IsRequired,
                IsReviewNeeded = request.IsReviewNeeded
            };
        }

        public CollectionStepDto Map(CoreDtos.CollectionStepDto stepDto, object collectedData, Guid[] relatedTasks)
        {
            if (stepDto == null)
                throw new ArgumentNullException(nameof(stepDto));

            var xPathDetails = _xPathParser.Parse(stepDto.XPath);
            var stepVariant = _stepVariantMapper.Map(xPathDetails);
            var stepData = _stepDataMapper.Map(xPathDetails, collectedData);

            return new CollectionStepDto
            {
                Id = stepDto.Id,
                Variant = stepVariant,
                State = stepDto.State,
                ReviewResult = stepDto.ReviewResult,
                IsRequired = stepDto.IsRequired,
                IsReviewNeeded = stepDto.IsReviewNeeded,
                Data = stepData,
                RelatedTasks = relatedTasks ?? Array.Empty<Guid>(),
                RequestedAt = stepDto.RequestedAt,
                UpdatedAt = stepDto.UpdatedAt
            };
        }

        public CoreDtos.CollectionStepPatch Map(UpdateCollectionStepRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new CoreDtos.CollectionStepPatch
            {
                IsRequired = request.IsRequired,
                IsReviewNeeded = request.IsReviewNeeded
            };
        }

        private static string BuildPersonalDetailsXpath(PersonalDetailsCollectionStepRequest request) =>
            request.PersonalProperty switch
            {
                PersonalDetailsProperty.FirstName => XPathes.FirstName,
                PersonalDetailsProperty.LastName => XPathes.LastName,
                PersonalDetailsProperty.Birthdate => XPathes.Birthdate,
                PersonalDetailsProperty.ResidenceAddress => XPathes.ResidenceAddress,
                PersonalDetailsProperty.Nationality => XPathes.PersonalNationality,
                PersonalDetailsProperty.Email => XPathes.Email,
                PersonalDetailsProperty.FullName => XPathes.FullName,
                _ => throw new ArgumentOutOfRangeException(nameof(request.PersonalProperty), request.PersonalProperty, "Unsupported property type.")
            };

        private static string BuildVerificationDetailsXpath(VerificationDetailsCollectionStepRequest request) =>
            request.VerificationProperty switch
            {
                VerificationDetailsProperty.IpAddress => XPathes.IpAddress,
                VerificationDetailsProperty.TaxResidence => XPathes.TaxResidence,
                VerificationDetailsProperty.RiskLevel => XPathes.RiskLevel,
                VerificationDetailsProperty.IdDocumentNumber => XPathes.IdDocumentNumber,
                VerificationDetailsProperty.Tin => XPathes.Tin,
                VerificationDetailsProperty.Nationality => XPathes.VerifiedNationality,
                VerificationDetailsProperty.IsPep => XPathes.IsPep,
                VerificationDetailsProperty.IsSanctioned => XPathes.IsSanctioned,
                VerificationDetailsProperty.IsAdverseMedia => XPathes.IsAdverseMedia,
                VerificationDetailsProperty.Turnover => XPathes.Turnover,
                VerificationDetailsProperty.PoiIssuingCountry => XPathes.PoiIssuingCountry,
                VerificationDetailsProperty.PlaceOfBirth => XPathes.PlaceOfBirth,
                VerificationDetailsProperty.ComprehensiveIndex => XPathes.ComprehensiveIndex,
                VerificationDetailsProperty.IsIpMatched => XPathes.IsIpMatched,
                VerificationDetailsProperty.ResolvedCountryCode => XPathes.ResolvedCountryCode,
                _ => throw new ArgumentOutOfRangeException(nameof(request.VerificationProperty), request.VerificationProperty, "Unsupported property type.")
            };

        private static string BuildDocumentXPath(DocumentCollectionStepRequest request)
        {
            var documentCategory = request.DocumentCategory;
            var documentType = request.DocumentType;
            return new DocumentXPath(documentCategory, documentType);
        }

        private static SurveyXPath BuildSurveyXPath(SurveyCollectionStepRequest request) =>
            new SurveyXPath(request.TemplateId);
    }
}