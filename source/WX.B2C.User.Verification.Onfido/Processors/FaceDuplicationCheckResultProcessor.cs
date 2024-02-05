using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Models;
using WX.B2C.User.Verification.Onfido.Processors.Validators;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using static WX.B2C.User.Verification.Provider.Contracts.Constants;
using CheckProcessingResult = WX.B2C.User.Verification.Provider.Contracts.Models.CheckProcessingResult;

namespace WX.B2C.User.Verification.Onfido.Processors
{
    internal interface IFaceDuplicationCheckResultProcessor
    {
        Task<CheckProcessingResult> ProcessAsync(Check check, IList<Report> reports);
    }

    internal class FaceDuplicationCheckResultProcessor : IFaceDuplicationCheckResultProcessor
    {
        private readonly KnownFacesReportValidator _validator;

        public FaceDuplicationCheckResultProcessor(KnownFacesReportValidator validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<CheckProcessingResult> ProcessAsync(Check check, IList<Report> reports)
        {
            var knownFacesReports = reports.OfType<KnownFacesReport>().ToArray();
            if (knownFacesReports.Length > 1)
                throw new CheckProcessingException(ErrorCodes.UnsupportedFunctionality, "Multiple reports are not supported.");

            var report = knownFacesReports.SingleOrDefault();
            if (report is null)
                throw new CheckProcessingException(ErrorCodes.ProviderInvalidResponse, "Known faces report was not found.");

            if (report.Result == CheckResult.Clear)
                return CheckProcessingResult.Passed(new FaceDuplicationOutputData());

            var validationResult = await _validator.ValidateAsync(report);
            var (decision, failures) = validationResult.IsValid
                ? (CheckDecisions.Consider, null)
                : (validationResult.GetDecision(), validationResult.GetFailures());

            var outputData = new FaceDuplicationOutputData
            {
                CheckResult = check.Result,
                ReportResult = report.Result,
                Failures = failures
            };
            return CheckProcessingResult.Failed(outputData, decision);
        }
    }
}
