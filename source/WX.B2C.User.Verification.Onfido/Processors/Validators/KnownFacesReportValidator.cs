using FluentValidation;
using FluentValidation.Results;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Constants;

namespace WX.B2C.User.Verification.Onfido.Processors.Validators
{
    internal class KnownFacesReportValidator : BaseReportValidator<KnownFacesReport>
    {
        public KnownFacesReportValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(report => report.Breakdown)
                .ImageIntegrityClear()
                .WithErrorCode(CheckDecisions.Resubmit)
                .WithErrorState();

            RuleFor(report => report.Breakdown)
                .NoPreviouslySeenFaces()
                .WithErrorCode(CheckDecisions.DuplicateAccount)
                .WithErrorState();

            RuleFor(report => report.Result)
                .Must(result => result.IsClearResult())
                .WithErrorCode(CheckDecisions.Consider)
                .WithState(_ => new[] { KnownFacesReportErrorCodes.NotClearReportResult });
        }

        protected override bool PreValidate(ValidationContext<KnownFacesReport> context, ValidationResult result)
        {
            if (context.InstanceToValidate?.Breakdown is null)
            {
                var error = new ValidationFailure(
                    KnownFacesReportErrorCodes.InvalidReport,
                    "Known faces report must not be null.");

                result.Errors.Add(error);
                return false;
            }

            return true;
        }
    }
}
