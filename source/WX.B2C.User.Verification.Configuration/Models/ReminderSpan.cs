using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Configuration.Models
{
    public class ReminderSpan
    {
        public int Value { get; set; }

        public IntervalUnit Unit { get; set; }

        public int Order { get; set; }
    }
}