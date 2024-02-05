using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class CheckProcessingResultDto
    {
        public CheckResult Result { get; set; }

        public string Decision { get; set; }

        public string OutputData { get; set; }
    }
}
