using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using WX.B2C.User.Verification.Core.Contracts.Triggers;

namespace WX.B2C.User.Verification.Infrastructure.Communication.Actors
{
    public interface ITriggerActor : IActor
    {
        Task ScheduleAsync(Guid triggerVariantId, Guid userId, Guid applicationId);

        Task UnscheduleAsync(Guid triggerId);

        Task FireAsync(Guid triggerId, TriggerContextDto context);

        Task CompleteAsync(Guid triggerId);
    }
}