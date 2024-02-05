using System.Collections.Generic;
using FluentValidation;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Constants;

namespace WX.B2C.User.Verification.Onfido.Processors.Validators
{
    internal static class FacialSimilarityReportValidatorExtensions
    {
        public static IRuleBuilderOptions<FacialSimilarityPhotoReport, FacialSimilarityPhotoReportBreakdown> GoodImageQuality(
            this IRuleBuilder<FacialSimilarityPhotoReport, FacialSimilarityPhotoReportBreakdown> ruleBuilder) =>
            ruleBuilder.ValidateBreakdown(
                breakdown =>
                {
                    var errors = new List<string>();

                    if (breakdown is null) return errors;

                    if (!breakdown.VisualAuthenticity.IsClearResult())
                        errors.Add(FacialSimilarityReportErrorCodes.CautionVisualAuthenticity);

                    if (!breakdown.ImageIntegrity.IsClearResult())
                        errors.Add(FacialSimilarityReportErrorCodes.CautionImageIntegrity);

                    return errors;
                });

        public static IRuleBuilderOptions<FacialSimilarityPhotoReport, FacialSimilarityPhotoReportBreakdown> NoFaceMatches(
            this IRuleBuilder<FacialSimilarityPhotoReport, FacialSimilarityPhotoReportBreakdown> ruleBuilder) =>
            ruleBuilder.ValidateBreakdown(
                breakdown =>
                {
                    var errors = new List<string>();

                    if (breakdown is null) return errors;

                    if (!breakdown.FaceComparison.IsClearResult())
                        errors.Add(FacialSimilarityReportErrorCodes.CautionFaceMatchesFound);

                    return errors;
                });

        public static IRuleBuilderOptions<FacialSimilarityVideoReport, FacialSimilarityVideoReportBreakdown> GoodImageQuality(
            this IRuleBuilder<FacialSimilarityVideoReport, FacialSimilarityVideoReportBreakdown> ruleBuilder) =>
            ruleBuilder.ValidateBreakdown(
                breakdown =>
                {
                    var errors = new List<string>();

                    if (breakdown is null) return errors;

                    if (!breakdown.VisualAuthenticity.IsClearResult())
                        errors.Add(FacialSimilarityReportErrorCodes.CautionVisualAuthenticity);

                    if (!breakdown.ImageIntegrity.IsClearResult())
                        errors.Add(FacialSimilarityReportErrorCodes.CautionImageIntegrity);

                    return errors;
                });

        public static IRuleBuilderOptions<FacialSimilarityVideoReport, FacialSimilarityVideoReportBreakdown> NoFaceMatches(
                this IRuleBuilder<FacialSimilarityVideoReport, FacialSimilarityVideoReportBreakdown> ruleBuilder) =>
            ruleBuilder.ValidateBreakdown(
                breakdown =>
                {
                    var errors = new List<string>();

                    if (breakdown is null) return errors;

                    if (!breakdown.FaceComparison.IsClearResult())
                        errors.Add(FacialSimilarityReportErrorCodes.CautionFaceMatchesFound);

                    return errors;
                });
    }
}