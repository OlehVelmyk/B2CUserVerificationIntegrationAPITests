using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos
{
    public class TinDto
    {
        public string Number { get; set; }

        public TinType Type { get; set; }
    }
}