using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Onfido.Models
{
    internal class FaceDuplicationCheckInputData : OnfidoCheckInputData
    {
        public FullNameDto FullName { get; set; }

        public DocumentDto Selfie { get; set; }
    }

    internal class FaceDuplicationOutputData : CheckOutputData
    {
        public string CheckResult { get; set; }

        public string ReportResult { get; set; }

        public IEnumerable<object> Failures { get; set; }
    }
}