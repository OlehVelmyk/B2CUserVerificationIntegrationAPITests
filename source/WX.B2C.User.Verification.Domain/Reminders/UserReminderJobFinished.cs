using System;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class UserReminderJobFinished : DomainEvent
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public static UserReminderJobFinished Create(Guid reminderId, Guid userId) =>
            new() { Id = reminderId, UserId = userId };
    }
}
