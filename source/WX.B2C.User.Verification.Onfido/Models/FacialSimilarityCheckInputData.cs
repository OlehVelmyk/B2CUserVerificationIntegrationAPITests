using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Onfido.Models
{
    internal class FacialSimilarityCheckInputData : OnfidoCheckInputData
    {
        public FullNameDto FullName { get; set; }

        public DocumentDto IdentityDocument { get; set; }

        public DocumentDto Selfie { get; set; }
    }

    internal class FacialSimilarityCheckOutputData : CheckOutputData
    {
        public string CheckResult { get; set; }

        public string ReportResult { get; set; }

        public object[] Failures { get; set; }
    }
}