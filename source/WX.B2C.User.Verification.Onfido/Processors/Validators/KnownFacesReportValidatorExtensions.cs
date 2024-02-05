using System.Collections.Generic;
using FluentValidation;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Constants;

namespace WX.B2C.User.Verification.Onfido.Processors.Validators
{
    internal static class KnownFacesReportValidatorExtensions
    {
        public static IRuleBuilderOptions<KnownFacesReport, KnownFacesReportBreakdown> ImageIntegrityClear(
            this IRuleBuilder<KnownFacesReport, KnownFacesReportBreakdown> ruleBuilder) =>
            ruleBuilder.ValidateBreakdown(
                breakdown =>
                {
                    var errors = new List<string>();

                    if (breakdown is null) return errors;

                    if (!breakdown.ImageIntegrity.IsClearResult())
                        errors.Add(KnownFacesReportErrorCodes.CautionImageIntegrity);

                    return errors;
                });

        public static IRuleBuilderOptions<KnownFacesReport, KnownFacesReportBreakdown> NoPreviouslySeenFaces(
            this IRuleBuilder<KnownFacesReport, KnownFacesReportBreakdown> ruleBuilder) =>
            ruleBuilder.ValidateBreakdown(
                breakdown =>
                {
                    var errors = new List<string>();

                    if (breakdown is null) return errors;

                    if (!breakdown.PreviouslySeenFaces.IsClearResult())
                        errors.Add(KnownFacesReportErrorCodes.CautionPreviouslySeenFaces);

                    return errors;
                });
    }
}