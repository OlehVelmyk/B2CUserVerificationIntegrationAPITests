using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.DataAccess.Entities;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface IReminderMapper
    {
        UserReminderDto Map(Reminder reminder);

        Reminder Map(UserReminderDto reminder);
    }

    internal class ReminderMapper : IReminderMapper
    {
        public UserReminderDto Map(Reminder reminder)
        {
            if (reminder is null)
                throw new ArgumentNullException(nameof(reminder));

            return new()
            {
                UserId = reminder.UserId,
                TargetId = reminder.TargetId,
                SentAt = reminder.SentAt
            };
        }

        public Reminder Map(UserReminderDto reminder)
        {
            if (reminder is null)
                throw new ArgumentNullException(nameof(reminder));

            var entity = new Reminder
            {
                Id = Guid.NewGuid(),
                TargetId = reminder.TargetId,
                UserId = reminder.UserId,
                SentAt = reminder.SentAt
            };
            return entity;
        }
    }
}
