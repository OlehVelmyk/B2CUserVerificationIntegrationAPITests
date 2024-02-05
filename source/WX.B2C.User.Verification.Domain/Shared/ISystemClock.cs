using System;

namespace WX.B2C.User.Verification.Domain.Shared
{
    public interface ISystemClock
    {
        DateTime GetDate();
    }

    public class DefaultSystemClock : ISystemClock
    {
        public DateTime GetDate() => DateTime.UtcNow;
    }
}
