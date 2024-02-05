using System.Collections.Generic;
using FluentValidation;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Constants;

namespace WX.B2C.User.Verification.Onfido.Processors.Validators
{
    internal static class DocumentReportValidationExtensions
    {
        public static IRuleBuilderOptions<DocumentReport, DocumentVisualAuthenticity> VisualAuthenticityClear(
            this IRuleBuilder<DocumentReport, DocumentVisualAuthenticity> ruleBuilder) =>
            ruleBuilder.ValidateBreakdown(
                visualAuthenticity =>
                {
                    var errors = new List<string>();

                    var breakdown = visualAuthenticity?.Breakdown;
                    if (breakdown is null) return errors;

                    if (!breakdown.DigitalTampering.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedVisualAuthenticationDigitalTampering);

                    if (!breakdown.FaceDetection.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedVisualAuthenticationFaceDetection);

                    if (!breakdown.Fonts.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedVisualAuthenticationFonts);

                    if (!breakdown.SecurityFeatures.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedVisualAuthenticationSecurityFeatures);

                    if (!breakdown.Template.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedVisualAuthenticationTemplate);

                    return errors;
                });

        public static IRuleBuilderOptions<DocumentReport, DocumentVisualAuthenticity> VisualAuthenticityClear2(
            this IRuleBuilder<DocumentReport, DocumentVisualAuthenticity> ruleBuilder) =>
            ruleBuilder.ValidateBreakdown(
                visualAuthenticity =>
                {
                    var errors = new List<string>();

                    var breakdown = visualAuthenticity?.Breakdown;
                    if (breakdown is null) return errors;

                    if (!breakdown.OriginalDocumentPresent.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedVisualAuthenticationOriginalDocumentPresent);

                    if (!breakdown.PictureFaceIntegrity.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedVisualAuthenticationPictureFaceIntegrity);

                    if (!breakdown.Other.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedVisualAuthenticationOther);

                    return errors;
                });

        public static IRuleBuilderOptions<DocumentReport, DocumentImageIntegrity> ImageIntegrityClear(
            this IRuleBuilder<DocumentReport, DocumentImageIntegrity> ruleBuilder) =>
            ruleBuilder.ValidateBreakdown(
                imageIntegrity =>
                {
                    var errors = new List<string>();

                    var breakdown = imageIntegrity?.Breakdown;
                    if (breakdown is null) return errors;

                    if (!breakdown.ColourPicture.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.CautionImageIntegrityColourPicture);

                    if (!breakdown.ConclusiveDocumentQuality.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.CautionImageIntegrityConclusiveDocumentQuality);

                    if (!breakdown.SupportedDocument.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.CautionImageIntegritySupportedDocument);

                    if (!breakdown.ImageQuality.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.CautionImageIntegrityImageQuality);

                    return errors;
                });

        public static IRuleBuilderOptions<DocumentReport, DocumentDataComparison> DataComparisonClear(
            this IRuleBuilder<DocumentReport, DocumentDataComparison> ruleBuilder) =>
            ruleBuilder.ValidateBreakdown(
                dataComparison =>
                {
                    var errors = new List<string>();

                    var breakdown = dataComparison?.Breakdown;
                    if (breakdown is null) return errors;

                    if (!breakdown.DocumentType.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.CautionDataComparisonDocumentType);

                    if (!breakdown.DateOfBirth.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.CautionDataComparisonDateOfBirth);

                    if (!breakdown.LastName.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.CautionDataComparisonLastName);

                    if (!breakdown.FirstName.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.CautionDataComparisonFirstName);

                    return errors;
                });

        public static IRuleBuilderOptions<DocumentReport, DocumentDataValidation> DataValidationClear(
            this IRuleBuilder<DocumentReport, DocumentDataValidation> ruleBuilder) =>
            ruleBuilder.ValidateBreakdown(
                dataValidation =>
                {
                    var errors = new List<string>();

                    var breakdown = dataValidation?.Breakdown;
                    if (breakdown is null) return errors;

                    if (!breakdown.DocumentExpiration.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedDataValidationDocumentExpiration);

                    if (!breakdown.Mrz.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedDataValidationMrz);

                    if (!breakdown.ExpiryDate.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedDataValidationExpiryDate);

                    if (!breakdown.DateOfBirth.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedDataValidationDateOfBirth);

                    return errors;
                });

        public static IRuleBuilderOptions<DocumentReport, DocumentDataConsistency> DataConsistencyClear(
            this IRuleBuilder<DocumentReport, DocumentDataConsistency> ruleBuilder) =>
            ruleBuilder.ValidateBreakdown(
                dataConsistency =>
                {
                    var errors = new List<string>();

                    var breakdown = dataConsistency?.Breakdown;
                    if (breakdown is null) return errors;

                    if (!breakdown.DateOfBirth.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedDataConsistencyDateOfBirth);

                    if (!breakdown.LastName.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedDataConsistencyLastName);

                    if (!breakdown.FirstName.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedDataConsistencyFirstName);

                    if (!breakdown.DocumentType.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedDataConsistencyDocumentType);

                    if (!breakdown.DateOfExpiry.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedDataConsistencyDateOfExpiry);

                    return errors;
                });

        public static IRuleBuilderOptions<DocumentReport, DocumentCompromisedDocument> CompromisedDocumentClear(
            this IRuleBuilder<DocumentReport, DocumentCompromisedDocument> ruleBuilder) =>
            ruleBuilder.ValidateBreakdown(
                compromisedDocument =>
                {
                    var errors = new List<string>();

                    if (!compromisedDocument.IsClearResult())
                        errors.Add(DocumentReportErrorCodes.SuspectedCompromisedDocumentsDocumentNotFound);

                    return errors;
                });
    }
}