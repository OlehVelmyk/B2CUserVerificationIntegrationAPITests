using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Provider.Services.System
{
    public class TaxResidenceInputData
    {
        public string[] TaxResidence { get; set; }
    }

    internal class TaxResidenceOutputData : CheckOutputData
    {
        public string[] MatchedCountries { get; set; }
    }
}