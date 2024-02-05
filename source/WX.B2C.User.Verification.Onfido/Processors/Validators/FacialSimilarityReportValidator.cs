using FluentValidation;
using FluentValidation.Results;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Constants;

namespace WX.B2C.User.Verification.Onfido.Processors.Validators
{
    internal class FacialSimilarityReportValidator : BaseReportValidator<Report>
    {
        public FacialSimilarityReportValidator()
        {
            RuleFor(report => report)
                .SetInheritanceValidator(validator =>
                {
                    validator.Add(new FacialSimilarityPhotoReportValidator());
                    validator.Add(new FacialSimilarityVideoReportValidator());
                });
        }
    }

    internal class FacialSimilarityPhotoReportValidator : BaseReportValidator<FacialSimilarityPhotoReport>
    {
        public FacialSimilarityPhotoReportValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(report => report.Breakdown)
                 .GoodImageQuality()
                 .WithErrorCode(CheckDecisions.Resubmit)
                 .WithErrorState();

            RuleFor(report => report.Breakdown)
                .NoFaceMatches()
                .WithErrorCode(CheckDecisions.Resubmit)
                .WithErrorState();
        }

        protected override bool PreValidate(ValidationContext<FacialSimilarityPhotoReport> context, ValidationResult result)
        {
            if (context.InstanceToValidate?.Breakdown is null)
            {
                var error = new ValidationFailure(
                    FacialSimilarityReportErrorCodes.InvalidReport,
                    "Facial similarity report must not be null.");

                result.Errors.Add(error);
                return false;
            }

            return true;
        }
    }

    internal class FacialSimilarityVideoReportValidator : BaseReportValidator<FacialSimilarityVideoReport>
    {
        public FacialSimilarityVideoReportValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(report => report.Breakdown)
                .GoodImageQuality()
                .WithErrorCode(CheckDecisions.Resubmit)
                .WithErrorState();

            RuleFor(report => report.Breakdown)
                .NoFaceMatches()
                .WithErrorCode(CheckDecisions.Consider)
                .WithErrorState();
        }

        protected override bool PreValidate(ValidationContext<FacialSimilarityVideoReport> context, ValidationResult result)
        {
            if (context.InstanceToValidate?.Breakdown is null)
            {
                var error = new ValidationFailure(
                    FacialSimilarityReportErrorCodes.InvalidReport,
                    "Facial similarity report must not be null.");

                result.Errors.Add(error);
                return false;
            }

            return true;
        }
    }
}
