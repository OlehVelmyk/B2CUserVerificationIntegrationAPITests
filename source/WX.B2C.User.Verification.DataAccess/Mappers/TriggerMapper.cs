using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Monitoring;
using WX.B2C.User.Verification.Domain.Triggers;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface ITriggerMapper
    {
        Trigger Map(Entities.Trigger entity);

        void Update(Trigger source, Entities.Trigger target);

        Entities.Trigger Map(Trigger trigger);

        TriggerDto MapToDto(Entities.Trigger entity);
    }

    internal class TriggerMapper : ITriggerMapper
    {
        public Trigger Map(Entities.Trigger entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new(entity.Id,
                       entity.VariantId,
                       entity.UserId,
                       entity.ApplicationId,
                       entity.State,
                       entity.ScheduleDate,
                       entity.FiringDate,
                       entity.UnscheduleDate,
                       entity.Context);
        }

        public void Update(Trigger source, Entities.Trigger target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.VariantId = source.VariantId;
            target.UserId = source.UserId;
            target.ApplicationId = source.ApplicationId;
            target.ScheduleDate = source.ScheduleDate;
            target.FiringDate = source.FiringDate;
            target.UnscheduleDate = source.UnscheduleDate;
            target.State = source.State;
            target.Context = source.Context == null ? null : new Dictionary<string, object>(source.Context);
        }

        public Entities.Trigger Map(Trigger trigger)
        {
            if (trigger == null)
                throw new ArgumentNullException(nameof(trigger));

            return new()
            {
                Id = trigger.Id,
                VariantId = trigger.VariantId,
                UserId = trigger.UserId,
                ApplicationId = trigger.ApplicationId,
                ScheduleDate = trigger.ScheduleDate,
                FiringDate = trigger.FiringDate,
                UnscheduleDate = trigger.UnscheduleDate,
                State = trigger.State,
                Context = trigger.Context == null ? null : new Dictionary<string, object>(trigger.Context)
        };
        }

        public TriggerDto MapToDto(Entities.Trigger entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new()
            {
                TriggerId = entity.Id,
                VariantId = entity.VariantId,
                UserId = entity.UserId,
                ApplicationId = entity.ApplicationId,
                State = entity.State
            };
        }
    }
}