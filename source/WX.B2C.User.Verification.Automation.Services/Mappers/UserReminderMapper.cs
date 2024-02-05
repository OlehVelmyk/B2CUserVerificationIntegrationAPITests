using System;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Triggers;

namespace WX.B2C.User.Verification.Automation.Services.Mappers
{
    internal interface IUserReminderMapper
    {
        JobParametersDto Map(Guid userId, Guid stepId);

        JobRequestDto Map(Guid stepId, Guid userId, DateTime fireAt);
    }

    internal class UserReminderMapper : IUserReminderMapper
    {
        public JobParametersDto Map(Guid stepId, Guid userId) =>
            new()
            {
                JobName = JobConstants.UserReminderJob,
                Parameters = MapParameters(stepId, userId)
            };
        
        public JobRequestDto Map(Guid stepId, Guid userId, DateTime fireAt) =>
            new()
            {
                JobName = JobConstants.UserReminderJob,
                StartAt = fireAt,
                Parameters = MapParameters(stepId, userId)
            };

        private JobParameterDto[] MapParameters(Guid userId, Guid stepId) =>
            new[]
            {
                new JobParameterDto
                {
                    Name = JobConstants.ReminderId,
                    Value = stepId
                },
                new JobParameterDto
                {
                    Name = JobConstants.UserId,
                    Value = userId
                }
            };
    }
}
