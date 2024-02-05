using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Models;
using WX.B2C.User.Verification.Onfido.Processors.Validators;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using static WX.B2C.User.Verification.Provider.Contracts.Constants;
using CheckDecisions = WX.B2C.User.Verification.Domain.Checks.CheckDecisions;

namespace WX.B2C.User.Verification.Onfido.Processors
{
    internal interface IFacialSimilarityCheckResultProcessor
    {
        Task<CheckProcessingResult> ProcessAsync(Check check, IList<Report> reports);
    }

    internal class FacialSimilarityCheckResultProcessor : IFacialSimilarityCheckResultProcessor
    {
        private readonly FacialSimilarityReportValidator _reportValidator;

        public FacialSimilarityCheckResultProcessor(FacialSimilarityReportValidator reportValidator)
        {
            _reportValidator = reportValidator ?? throw new ArgumentNullException(nameof(reportValidator));
        }

        public async Task<CheckProcessingResult> ProcessAsync(Check check, IList<Report> reports)
        {
            var facialSimilarityReports = reports.Where(IsFacialSimilarityReport).ToArray();
            if (facialSimilarityReports.Length > 1)
                throw new CheckProcessingException(ErrorCodes.UnsupportedFunctionality, "Multiple reports are not supported.");

            var report = facialSimilarityReports.SingleOrDefault();
            if (report is null)
                throw new CheckProcessingException(ErrorCodes.ProviderInvalidResponse, "Facial similarity report was not found.");

            if (report.Result == ReportResult.Clear)
                return CheckProcessingResult.Passed(new FacialSimilarityCheckOutputData());

            var validationResult = await _reportValidator.ValidateAsync(report);

            var (decision, failures) = validationResult.IsValid
                ? (CheckDecisions.Consider, null)
                : (validationResult.GetDecision(), validationResult.GetFailures());

            var outputData = new FacialSimilarityCheckOutputData
            {
                CheckResult = check.Result,
                ReportResult = report.Result,
                Failures = failures
            };
            return CheckProcessingResult.Failed(outputData, decision);

            static bool IsFacialSimilarityReport(Report report) =>
                report is FacialSimilarityPhotoReport or FacialSimilarityVideoReport;
        }
    }
}
