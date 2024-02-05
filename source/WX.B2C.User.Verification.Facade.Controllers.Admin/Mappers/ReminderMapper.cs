using System;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Triggers;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers
{
    public interface IReminderMapper
    {
        ReminderDto Map(UserReminderDto reminder);
        
        ActiveReminderDto Map(JobTriggerDto reminder);

        JobParametersDto MapToJob(Guid userId);
    }

    internal class ReminderMapper : IReminderMapper
    {
        public ReminderDto Map(UserReminderDto reminder)
        {
            if(reminder is null)
                throw new ArgumentNullException(nameof(reminder));

            return new()
            {
                UserId = reminder.UserId,
                TargetId = reminder.TargetId,
                SentAt = reminder.SentAt
            };
        }

        public ActiveReminderDto Map(JobTriggerDto reminder)
        {
            if (reminder == null)
                throw new ArgumentNullException(nameof(reminder));

            return new ActiveReminderDto
            {
                UserId = new Guid(reminder.JobKey),
                TargetId = new Guid(reminder.TriggerId),
                SendAt = reminder.NextFireTime
            };
        }

        public JobParametersDto MapToJob(Guid userId) =>
            new()
            {
                JobName = JobConstants.UserReminderJob,
                Parameters = new []
                {
                  new JobParameterDto
                  {
                      Name = JobConstants.UserId,
                      Value = userId
                  }  
                }
            };
    }
}
