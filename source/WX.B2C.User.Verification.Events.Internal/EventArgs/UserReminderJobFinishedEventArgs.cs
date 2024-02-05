using System;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class UserReminderJobFinishedEventArgs : System.EventArgs
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public static UserReminderJobFinishedEventArgs Create(Guid reminderId, Guid userId) =>
            new UserReminderJobFinishedEventArgs() { Id = reminderId, UserId = userId };
    }
}
