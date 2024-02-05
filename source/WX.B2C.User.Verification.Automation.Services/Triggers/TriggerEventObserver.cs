using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Automation;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Monitoring;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services.Triggers
{
    internal class TriggerEventObserver : ITriggerEventObserver
    {
        private readonly ITriggerManagerFactory _triggerManagerFactory;
        private readonly IApplicationStorage _applicationStorage;
        private readonly IMonitoringPolicyProvider _monitoringPolicyProvider;
        private readonly ITriggerStorage _triggerStorage;
        private readonly ITriggerVariantStorage _triggerVariantStorage;

        public TriggerEventObserver(ITriggerManagerFactory triggerManagerFactory, 
                                    IApplicationStorage applicationStorage, 
                                    IMonitoringPolicyProvider monitoringPolicyProvider,
                                    ITriggerStorage triggerStorage,
                                    ITriggerVariantStorage triggerVariantStorage)
        {
            _triggerManagerFactory = triggerManagerFactory ?? throw new ArgumentNullException(nameof(triggerManagerFactory));
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
            _monitoringPolicyProvider = monitoringPolicyProvider ?? throw new ArgumentNullException(nameof(monitoringPolicyProvider));
            _triggerStorage = triggerStorage ?? throw new ArgumentNullException(nameof(triggerStorage));
            _triggerVariantStorage = triggerVariantStorage ?? throw new ArgumentNullException(nameof(triggerVariantStorage));
        }

        public async Task OnTriggerReadyToFire(Guid triggerId)
        {
            var trigger = await _triggerStorage.GetAsync(triggerId);
            var triggerVariant = await _triggerVariantStorage.GetAsync(trigger.VariantId);
            var manager = _triggerManagerFactory.Create(trigger.UserId);
            await manager.TryFireAsync(trigger, triggerVariant);
        }

        public async Task OnTriggerScheduled(Guid triggerVariantId, Guid triggerId)
        {
            var triggerVariant = await _triggerVariantStorage.GetAsync(triggerVariantId);
            if (triggerVariant.IsScheduled)
                return;

            var trigger = await _triggerStorage.GetAsync(triggerId);
            var manager = _triggerManagerFactory.Create(trigger.UserId);
            await manager.TryFireAsync(trigger, triggerVariant);
        }

        public async Task OnApplicationStateChanged(Guid userId,
                                                    Guid applicationId,
                                                    ApplicationState previousState,
                                                    ApplicationState newState)
        {
            var manager = _triggerManagerFactory.Create(userId);
            if (previousState is ApplicationState.Applied or ApplicationState.Approved)
                await UnscheduleTriggersAsync(applicationId, manager);

            if (newState == ApplicationState.Approved)
                await ScheduleMonitoringTriggersAsync(userId, manager);
        }

        public async Task OnDetailsChanged(Guid userId)
        {
            var manager = _triggerManagerFactory.Create(userId);
            await ScheduleOnboardingTriggersAsync(userId, manager);
            await ScheduleMonitoringTriggersAsync(userId, manager);
        }

        public async Task OnActionsRequested(Guid userId, string[] actions, Guid triggersPolicyId)
        {
            var application = await _applicationStorage.FindAsync(userId, ProductType.WirexBasic);
            if (application == null)
                return;

            var applicationId = application.Id;
            var variants = await _triggerVariantStorage.FindAsync(triggersPolicyId);
            var triggers = await _triggerStorage.GetAllAsync(applicationId);
            var manager = _triggerManagerFactory.Create(userId);

            if (actions.Contains(TriggerActions.Unschedule))
                await manager.TryUnscheduleAsync(variants, triggers);
            if (actions.Contains(TriggerActions.Schedule))
                await manager.TryScheduleAsync(variants, triggers, applicationId);
            if (actions.Contains(TriggerActions.Fire))
                await manager.TryFireAsync(variants, triggers);
        }

        /// <summary>
        /// TODO WRXB-10546 Remove in phase 2 when all users will be migrated
        /// </summary>
        public async Task OnApplicationAutomated(Guid userId, Guid applicationId)
        {
            var manager = _triggerManagerFactory.Create(userId);
            await ScheduleOnboardingTriggersAsync(userId, manager);
            await ScheduleMonitoringTriggersAsync(userId, manager);
        }

        public async Task OnTriggerCompleted(Guid userId, Guid applicationId, Guid variantId)
        {
            var trigger = await _triggerVariantStorage.GetAsync(variantId);
            var manager = _triggerManagerFactory.Create(userId);
            await manager.TryScheduleIterative(trigger, applicationId);
        }

        private async Task UnscheduleTriggersAsync(Guid applicationId, ITriggerManager manager)
        {
            var triggers = await _triggerStorage.GetAllAsync(applicationId);
            await manager.TryUnscheduleAsync(triggers);
        }

        private async Task ScheduleOnboardingTriggersAsync(Guid userId, ITriggerManager manager)
        {
            var appliedApplications = await _applicationStorage.FindAsync(userId, ApplicationState.Applied);
            var automatedApplications = ReadyForAutomation(appliedApplications);
            await automatedApplications.Foreach(application => RescheduleTriggers(application.Id, application.PolicyId, manager));
        }

        private async Task ScheduleMonitoringTriggersAsync(Guid userId, ITriggerManager manager)
        {
            //TODO PHASE 2 how to guess on which application should monitoring work?
            var application = await _applicationStorage.FindAsync(userId, ProductType.WirexBasic);
            if (application is not { State: ApplicationState.Approved })
                return;

            //TODO WRXB-10546 Remove in phase 2 when all users will be migrated
            if (!application.IsAutomating)
                return;
            
            var policy = await _monitoringPolicyProvider.FindAsync(userId);
            if (policy == null)
                return;

            await RescheduleTriggers(application.Id, policy.Id, manager);
        }

        private async Task RescheduleTriggers(Guid applicationId,
                                              Guid triggerPolicyId,
                                              ITriggerManager manager)
        {
            var variants = await _triggerVariantStorage.FindAsync(triggerPolicyId);
            var triggers = await _triggerStorage.GetAllAsync(applicationId);
            var unscheduling = manager.TryUnscheduleAsync(variants, triggers);
            var scheduling = manager.TryScheduleAsync(variants, triggers, applicationId);
            var firing = manager.TryFireAsync(variants, triggers);
            await Task.WhenAll(unscheduling, scheduling, firing);
        }
        
        /// <summary>
        /// TODO WRXB-10546 Remove in phase 2 when all users will be migrated
        /// </summary>
        private static IReadOnlyCollection<ApplicationDto> ReadyForAutomation(IEnumerable<ApplicationDto> applications) =>
            applications.Where(dto => dto.IsAutomating).ToArray();
    }
}