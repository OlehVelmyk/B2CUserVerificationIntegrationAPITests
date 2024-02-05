using System;
using WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Extensions
{
    internal static class CollectionStepExtensions
    {
        public static bool IsRequested(this CollectionStepDto collectionStep) =>
            collectionStep?.State == CollectionStepState.Requested;

        public static CollectionStepRequest Map(this CollectionStepVariantDto stepVariant) =>
            stepVariant switch
            {
                DocumentCollectionStepVariantDto documentVariant => Map(documentVariant),
                PersonalDetailsCollectionStepVariantDto personalDetailsVariant => Map(personalDetailsVariant),
                VerificationDetailsCollectionStepVariantDto verificationDetailsVariant => Map(verificationDetailsVariant),
                SurveyCollectionStepVariantDto surveyVariant => Map(surveyVariant),
                _ => throw new ArgumentOutOfRangeException()
            };

        private static CollectionStepRequest Map(DocumentCollectionStepVariantDto stepVariant) =>
            new DocumentCollectionStepRequest
            {
                Type = CollectionStepType.Document,
                DocumentCategory = stepVariant.DocumentCategory,
                DocumentType = stepVariant.DocumentType,
                Reason = nameof(CollectionStepExtensions)
            };

        private static CollectionStepRequest Map(PersonalDetailsCollectionStepVariantDto stepVariant) =>
            new PersonalDetailsCollectionStepRequest
            {
                Type = CollectionStepType.PersonalDetails,
                PersonalProperty = stepVariant.Property,
                Reason = nameof(CollectionStepExtensions)
            };

        private static CollectionStepRequest Map(VerificationDetailsCollectionStepVariantDto stepVariant) =>
            new VerificationDetailsCollectionStepRequest
            {
                Type = CollectionStepType.VerificationDetails,
                VerificationProperty = stepVariant.Property,
                Reason = nameof(CollectionStepExtensions)
            };

        private static CollectionStepRequest Map(SurveyCollectionStepVariantDto stepVariant) =>
            new SurveyCollectionStepRequest
            {
                Type = CollectionStepType.Survey,
                TemplateId = stepVariant.TemplateId,
                Reason = nameof(CollectionStepExtensions)
            };
    }
}
