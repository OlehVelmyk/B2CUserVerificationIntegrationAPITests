using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Conditions;
using WX.B2C.User.Verification.Core.Contracts.Monitoring;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Core.Contracts.Triggers;
using WX.B2C.User.Verification.Domain.Triggers;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services.Triggers
{
    internal interface ITriggerManager
    {
        /// <summary>
        /// Unschedule all scheduled triggers
        /// </summary>
        Task TryUnscheduleAsync(TriggerDto[] triggers);

        /// <summary>
        /// Unschedule all scheduled triggers from set of variants where preconditions are not satisfied.
        /// </summary>
        Task TryUnscheduleAsync(TriggerVariantDto[] variants, TriggerDto[] triggers);

        /// <summary>
        /// Schedule all not scheduled or executed triggers from set of variants where preconditions are satisfied now.
        /// </summary>
        Task TryScheduleAsync(TriggerVariantDto[] variants, TriggerDto[] triggers, Guid applicationId);

        /// <summary>
        /// Fire all scheduled triggers from set of variants where conditions are satisfied.
        /// </summary>
        Task TryFireAsync(TriggerVariantDto[] variants, TriggerDto[] triggers);

        /// <summary>
        /// Fire trigger if conditions are satisfied and trigger is scheduled.
        /// </summary>
        Task TryFireAsync(TriggerDto triggerDto, TriggerVariantDto triggerVariantDto);

        Task TryScheduleIterative(TriggerVariantDto variant, Guid applicationId);
    }

    /// <summary>
    /// TODO PHASE2 refactor to have better interface.
    /// </summary>
    internal class TriggersManager : ITriggerManager
    {
        private readonly Guid _userId;
        private readonly ITriggerConditionService _conditionService;
        private readonly ITriggerService _triggerService;
        private readonly ITriggerStorage _triggerStorage;

        public TriggersManager(Guid userId,
                               ITriggerConditionService conditionService,
                               ITriggerService triggerService,
                               ITriggerStorage triggerStorage)
        {
            _userId = userId;
            _conditionService = conditionService ?? throw new ArgumentNullException(nameof(conditionService));
            _triggerService = triggerService ?? throw new ArgumentNullException(nameof(triggerService));
            _triggerStorage = triggerStorage ?? throw new ArgumentNullException(nameof(triggerStorage));
        }

        public Task TryUnscheduleAsync(TriggerDto[] triggers)
        {
            var scheduledTriggers = triggers.Where(dto => dto.State == TriggerState.Scheduled);
            return scheduledTriggers.Foreach(t => _triggerService.UnscheduleAsync(t.TriggerId));
        }

        public async Task TryUnscheduleAsync(TriggerVariantDto[] variants, TriggerDto[] triggers)
        {
            var triggersToUnschedule = await GetTriggersToUnscheduleAsync(variants, triggers);
            await triggersToUnschedule.Foreach(t => _triggerService.UnscheduleAsync(t.TriggerId));
        }

        public async Task TryScheduleAsync(TriggerVariantDto[] variants, TriggerDto[] triggers, Guid applicationId)
        {
            var triggersToSchedule = await GetTriggersToScheduleAsync(variants, triggers);
            await triggersToSchedule.Foreach(variant => _triggerService.ScheduleAsync(variant.Id, _userId, applicationId));
        }

        public async Task TryFireAsync(TriggerVariantDto[] variants, TriggerDto[] triggers)
        {
            var triggersToFire = await GetTriggersToFireAsync(variants, triggers);
            await triggersToFire.Foreach(FireTriggerAsync);
        }

        public async Task TryFireAsync(TriggerDto triggerDto, TriggerVariantDto triggerVariantDto)
        {
            if (triggerDto == null)
                throw new ArgumentNullException(nameof(triggerDto));
            if (triggerVariantDto == null)
                throw new ArgumentNullException(nameof(triggerVariantDto));

            if (triggerDto.State != TriggerState.Scheduled)
                return;

            var applicationId = triggerDto.ApplicationId;

            var context = await PrepareTriggerContextAsync(triggerVariantDto, applicationId);
            if (triggerVariantDto.HasConditions)
            {
                var isSatisfied = await IsSatisfiedAsync(triggerVariantDto.Conditions, context);
                if (!isSatisfied)
                    return;
            }

            var triggerContextDto = TriggerContextDto.Create(context);
            var triggerToFire = TriggerToFire.Create(triggerDto, triggerVariantDto, triggerContextDto);
            await FireTriggerAsync(triggerToFire);
        }

        public async Task TryScheduleIterative(TriggerVariantDto variant, Guid applicationId)
        {
            if (variant == null)
                throw new ArgumentNullException(nameof(variant));

            if (!variant.Iterative)
                return;

            if (await IsPreconditionsSatisfied(variant))
                await _triggerService.ScheduleAsync(variant.Id, _userId, applicationId);
        }

        private async Task<bool> IsPreconditionsSatisfied(TriggerVariantDto variant) =>
            !variant.HasPreconditions || await IsSatisfiedAsync(variant.Preconditions, new Dictionary<string, object>());

        private Task FireTriggerAsync(TriggerToFire triggerToFire) =>
            _triggerService.FireAsync(triggerToFire.TriggerDto.TriggerId, triggerToFire.TriggerContextDto);

        private async Task<TriggerDto[]> GetTriggersToUnscheduleAsync(TriggerVariantDto[] variants, TriggerDto[] triggers)
        {
            var scheduledTriggers = triggers.Where(dto => dto.State == TriggerState.Scheduled);
            var triggersWithPreconditions = variants.Where(dto => dto.HasPreconditions).ToLookup(dto => dto.Id);

            var triggersToUnschedule = await scheduledTriggers.Where(async trigger =>
            {
                var variant = triggersWithPreconditions[trigger.VariantId].FirstOrDefault();
                if (variant == null)
                    return false;

                var context = new Dictionary<string, object>();
                var isSatisfied = await IsSatisfiedAsync(variant.Preconditions, context);
                return !isSatisfied;
            });

            return triggersToUnschedule.ToArray();
        }

        private async Task<TriggerVariantDto[]> GetTriggersToScheduleAsync(TriggerVariantDto[] variants, TriggerDto[] triggers)
        {
            var existingTriggers = triggers.ToLookup(dto => dto.VariantId);
            var triggersToSchedule = await variants.Where(async variant =>
            {
                var alreadyExists = existingTriggers[variant.Id].Any(trigger => trigger.State is not TriggerState.Unscheduled);
                if (alreadyExists)
                    return false;

                return await IsPreconditionsSatisfied(variant);
            });

            return triggersToSchedule.ToArray();
        }

        private async Task<TriggerToFire[]> GetTriggersToFireAsync(TriggerVariantDto[] variants, TriggerDto[] triggers)
        {
            var scheduledTriggers = triggers.Where(dto => dto.State == TriggerState.Scheduled);
            var variantsLookup = variants.Where(dto => !dto.IsScheduled).ToLookup(dto => dto.Id);

            var triggersToFire = new List<TriggerToFire>();
            foreach (var trigger in scheduledTriggers)
            {
                var variant = variantsLookup[trigger.VariantId].FirstOrDefault();
                if (variant == null)
                    continue;

                var context = await PrepareTriggerContextAsync(variant, trigger.ApplicationId);
                var canFire = !variant.HasConditions || await IsSatisfiedAsync(variant.Conditions, context);
                if (canFire)
                    triggersToFire.Add(TriggerToFire.Create(trigger, variant, TriggerContextDto.Create(context)));
            }

            return triggersToFire.ToArray();
        }

        private async Task<Dictionary<string, object>> PrepareTriggerContextAsync(TriggerVariantDto variant, Guid applicationId)
        {
            var context = new Dictionary<string, object>();

            if (!variant.HasConditions)
                return context;

            var previousContextProperties = _conditionService.PreviousContextProperties(variant.Conditions);
            if (previousContextProperties.Length == 0)
                return context;

            var previousContext = await _triggerStorage.FindLastContextAsync(variant.Id, applicationId);
            if (previousContext == null)
                return context;

            previousContextProperties.Foreach(property =>
            {
                if (previousContext.TryGetValue(property, out var value))
                    context.Add(property, value);
            });
            return context;
        }

        private async Task<bool> IsSatisfiedAsync(Condition[] conditions, Dictionary<string, object> context)
        {
            if (conditions == null)
                throw new ArgumentNullException(nameof(conditions));
            if (conditions.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(conditions));

            return await _conditionService.IsAnySatisfiedAsync(conditions, context);
        }

        private class TriggerToFire
        {
            private TriggerToFire(TriggerDto triggerDto, TriggerVariantDto triggerVariantDto, TriggerContextDto triggerContextDto)
            {
                TriggerDto = triggerDto;
                TriggerVariantDto = triggerVariantDto;
                TriggerContextDto = triggerContextDto;
            }

            public static TriggerToFire Create(TriggerDto triggerDto, TriggerVariantDto triggerVariantDto, TriggerContextDto triggerContextDto)
            {
                return new TriggerToFire(triggerDto, triggerVariantDto, triggerContextDto);
            }

            public TriggerDto TriggerDto { get; }

            public TriggerVariantDto TriggerVariantDto { get; }

            public TriggerContextDto TriggerContextDto { get; }
        }

    }
}