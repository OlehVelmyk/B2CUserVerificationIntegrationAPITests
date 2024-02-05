using WX.B2C.User.Verification.LexisNexis.Models;
using WX.B2C.User.Verification.Provider.Contracts.Models;
using WX.B2C.User.Verification.Provider.Contracts.Stubs;

namespace WX.B2C.User.Verification.LexisNexis.Factories
{
    public class RiskScreeningCheckProcessingResultFactory : PassedCheckProcessingResultFactory
    {
        protected override CheckOutputData CreateCheckOutput(CheckInputData inputData)
        {
            return new LexisNexisRiskScreeningOutputData
            {
                IsPep = false,
                IsSanctioned = false,
                IsAdverseMedia = false
            };
        }
    }
}