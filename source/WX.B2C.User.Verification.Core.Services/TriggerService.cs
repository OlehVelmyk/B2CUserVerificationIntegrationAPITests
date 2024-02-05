using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Repositories;
using WX.B2C.User.Verification.Core.Contracts.Triggers;
using WX.B2C.User.Verification.Core.Services.Utilities;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Domain.Triggers;

namespace WX.B2C.User.Verification.Core.Services
{
    internal class TriggerService : ITriggerService
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly ITriggerRepository _triggerRepository;

        public TriggerService(ITriggerRepository triggerRepository,
                              IEventPublisher eventPublisher)
        {
            _triggerRepository = triggerRepository ?? throw new ArgumentNullException(nameof(triggerRepository));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public Task ScheduleAsync(Guid triggerVariantId, Guid userId, Guid applicationId) =>
            AppCore.ApplyChangesAsync(() => GetOrCreateAsync(triggerVariantId, userId, applicationId),
                                      trigger => { trigger.Schedule(); },
                                      SaveAndPublishAsync);

        public Task UnscheduleAsync(Guid triggerId) =>
            AppCore.ApplyChangesAsync(() => GetAsync(triggerId),
                                      trigger => { trigger.Unschedule(); },
                                      SaveAndPublishAsync);

        public Task FireAsync(Guid triggerId, TriggerContextDto context) =>
            AppCore.ApplyChangesAsync(() => GetAsync(triggerId),
                                      trigger => { trigger.Fire(context); },
                                      SaveAndPublishAsync);

        public Task CompleteAsync(Guid triggerId) =>
            AppCore.ApplyChangesAsync(() => GetAsync(triggerId),
                                      trigger => { trigger.Complete(); },
                                      SaveAndPublishAsync);

        private async Task<Trigger> GetOrCreateAsync(Guid triggerVariantId, Guid userId, Guid applicationId)
        {
            var trigger = await _triggerRepository.FindNotFiredAsync(triggerVariantId, applicationId);
            return trigger ?? Trigger.Create(triggerVariantId, userId, applicationId);
        }

        private Task<Trigger> GetAsync(Guid triggerId) =>
            _triggerRepository.GetAsync(triggerId);

        private async Task SaveAndPublishAsync(Trigger task)
        {
            await _triggerRepository.SaveAsync(task);
            await _eventPublisher.PublishAsync(task.CommitEvents());
        }
    }
}