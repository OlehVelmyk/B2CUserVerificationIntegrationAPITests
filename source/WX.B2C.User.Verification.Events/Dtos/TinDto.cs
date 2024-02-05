using WX.B2C.User.Verification.Events.Enums;

namespace WX.B2C.User.Verification.Events.Dtos
{
    public class TinDto
    {
        public string Number { get; set; }

        public TinType Type { get; set; }
    }
}
