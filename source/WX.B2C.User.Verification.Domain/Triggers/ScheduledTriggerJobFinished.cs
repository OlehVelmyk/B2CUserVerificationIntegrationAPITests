using System;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Triggers
{
    public class ScheduledTriggerJobFinished : DomainEvent
    {
        private ScheduledTriggerJobFinished(Guid triggerId,
                                            Guid userId,
                                            DateTime finishingDate)
        {
            TriggerId = triggerId;
            UserId = userId;
            FinishingDate = finishingDate;
        }

        public static ScheduledTriggerJobFinished Create(Guid triggerId,
                                                         Guid userId,
                                                         DateTime finishedTime) =>
            new(triggerId, userId, finishedTime);

        public Guid TriggerId { get; }

        public Guid UserId { get; }

        public DateTime FinishingDate { get; }
    }
}