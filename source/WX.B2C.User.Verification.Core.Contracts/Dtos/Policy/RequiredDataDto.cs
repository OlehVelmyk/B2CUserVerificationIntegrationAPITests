using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Policy
{
    public class RequiredDataDto
    {
        public RequiredDataType Type { get; set; }

        public string Config { get; set; }
    }
}