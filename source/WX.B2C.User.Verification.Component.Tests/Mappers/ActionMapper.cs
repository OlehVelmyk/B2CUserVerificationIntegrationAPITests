using System;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Constants;
using ActionType = WX.B2C.User.Verification.Api.Public.Client.Models.ActionType;
using AdminModels = WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Component.Tests.Mappers
{
    internal class ActionMapper
    {
        public static Models.Enums.DocumentCategory Map(ActionType actionType) =>
            actionType switch
            {
                ActionType.Selfie          => Models.Enums.DocumentCategory.Selfie,
                ActionType.W9Form          => Models.Enums.DocumentCategory.Taxation,
                ActionType.ProofOfAddress  => Models.Enums.DocumentCategory.ProofOfAddress,
                ActionType.ProofOfFunds    => Models.Enums.DocumentCategory.ProofOfFunds,
                ActionType.ProofOfIdentity => Models.Enums.DocumentCategory.ProofOfIdentity,
                ActionType.Survey or
                    ActionType.TaxResidence or
                    ActionType.Tin => throw new NotSupportedException($"ActionType: {actionType} do not have corresponding document type"),
                _ => throw new ArgumentOutOfRangeException(nameof(actionType), actionType, "Unsupported action type.")
            };
        
        public static ActionType? Map(CollectionStepVariantDto variant) =>
            variant switch
            {
                PersonalDetailsCollectionStepVariantDto => null,
                SurveyCollectionStepVariantDto => ActionType.Survey,
                VerificationDetailsCollectionStepVariantDto v => Map(v),
                DocumentCollectionStepVariantDto d => Map(d),
                _ => throw new ArgumentOutOfRangeException(nameof(variant), $"Cannot map variant of type {variant.GetType().Name}")
            };

        private static ActionType? Map(VerificationDetailsCollectionStepVariantDto variant) =>
            variant.Property switch
            {
                VerificationDetailsProperty.TaxResidence => ActionType.TaxResidence,
                VerificationDetailsProperty.Tin          => ActionType.Tin,
                _                                        => null
            };

        private static ActionType? Map(DocumentCollectionStepVariantDto variant) =>
            (variant.DocumentCategory, variant.DocumentType) switch
            {
                (AdminModels.DocumentCategory.ProofOfIdentity, _)             => ActionType.ProofOfIdentity,
                (AdminModels.DocumentCategory.ProofOfAddress, _)              => ActionType.ProofOfAddress,
                (AdminModels.DocumentCategory.ProofOfFunds, _)                => ActionType.ProofOfFunds,
                (AdminModels.DocumentCategory.Selfie, _)                      => ActionType.Selfie,
                (AdminModels.DocumentCategory.Taxation, DocumentTypes.W9Form) => ActionType.W9Form,
                _                                                             => null
            };
    }
}
