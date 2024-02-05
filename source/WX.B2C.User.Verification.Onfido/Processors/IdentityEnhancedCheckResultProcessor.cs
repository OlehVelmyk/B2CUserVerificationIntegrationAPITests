using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Onfido.Client.Models;
using WX.B2C.User.Verification.Onfido.Models;
using WX.B2C.User.Verification.Provider.Contracts.Exceptions;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using static WX.B2C.User.Verification.Provider.Contracts.Constants;

namespace WX.B2C.User.Verification.Onfido.Processors
{
    internal interface IIdentityEnhancedCheckResultProcessor
    {
        CheckProcessingResult Process(Check check, IList<Report> reports);
    }

    public class IdentityEnhancedCheckResultProcessor : IIdentityEnhancedCheckResultProcessor
    {
        public CheckProcessingResult Process(Check check, IList<Report> reports)
        {
            var identityEnhancedReports = reports.OfType<IdentityEnhancedReport>().ToArray();
            if (identityEnhancedReports.Length > 1)
                throw new CheckProcessingException(ErrorCodes.UnsupportedFunctionality, "Multiple reports are not supported.");

            var report = identityEnhancedReports.SingleOrDefault();
            if (report is null)
                throw new CheckProcessingException(ErrorCodes.ProviderInvalidResponse, "Identity enhanced report was not found.");

            return report.Result == CheckResult.Clear
                ? CheckProcessingResult.Passed(new IdentityEnhancedCheckOutputData())
                : CheckProcessingResult.Failed(new IdentityEnhancedCheckOutputData());
        }
    }
}