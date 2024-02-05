using FluentValidation;
using FluentValidation.Results;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Constants;

namespace WX.B2C.User.Verification.Onfido.Processors.Validators
{
    internal class DocumentReportValidator : BaseReportValidator<DocumentReport>
    {
        public DocumentReportValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(report => report.Breakdown.VisualAuthenticity)
                .VisualAuthenticityClear()
                .WithErrorCode(CheckDecisions.Fraud)
                .WithErrorState();

            RuleFor(report => report.Breakdown.ImageIntegrity)
                .ImageIntegrityClear()
                .WithErrorCode(CheckDecisions.Resubmit)
                .WithErrorState();

            RuleFor(report => report.Breakdown.DataComparison)
                .DataComparisonClear()
                .WithErrorCode(CheckDecisions.Resubmit)
                .WithErrorState();

            RuleFor(report => report.Breakdown.DataValidation)
                .DataValidationClear()
                .WithErrorCode(CheckDecisions.Resubmit)
                .WithErrorState();

            RuleFor(report => report.Breakdown.DataConsistency)
                .DataConsistencyClear()
                .WithErrorCode(CheckDecisions.Resubmit)
                .WithErrorState();

            RuleFor(report => report.Breakdown.VisualAuthenticity)
                .VisualAuthenticityClear2()
                .WithErrorCode(CheckDecisions.Resubmit)
                .WithErrorState();

            RuleFor(report => report.Breakdown.CompromisedDocument)
                .CompromisedDocumentClear()
                .WithErrorCode(CheckDecisions.Resubmit)
                .WithErrorState();

            RuleFor(report => report.SubResult)
                .Must(subResult => subResult.IsClearResult())
                .WithErrorCode(CheckDecisions.Resubmit)
                .WithState(_ => new[] { DocumentReportErrorCodes.NotClearReportResult });
        }

        protected override bool PreValidate(ValidationContext<DocumentReport> context, ValidationResult result)
        {
            if (context.InstanceToValidate?.Breakdown is null)
            {
                var error = new ValidationFailure(
                    DocumentReportErrorCodes.InvalidReport,
                    "Document report must not be null.");

                result.Errors.Add(error);
                return false;
            }

            return true;
        }
    }
}