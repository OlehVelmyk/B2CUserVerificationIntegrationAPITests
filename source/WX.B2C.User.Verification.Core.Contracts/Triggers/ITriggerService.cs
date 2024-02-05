using System;
using System.Threading.Tasks;

namespace WX.B2C.User.Verification.Core.Contracts.Triggers
{
    public interface ITriggerService
    {
        Task ScheduleAsync(Guid triggerVariantId, Guid userId, Guid applicationId);

        Task UnscheduleAsync(Guid triggerId);

        Task FireAsync(Guid triggerId, TriggerContextDto context);

        Task CompleteAsync(Guid triggerId);
    }
}