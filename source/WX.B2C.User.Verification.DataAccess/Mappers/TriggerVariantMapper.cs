using System;
using System.Linq;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Monitoring;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Core.Contracts.Triggers.Configs;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;
using Schedule = WX.B2C.User.Verification.Core.Contracts.Monitoring.Schedule;

namespace WX.B2C.User.Verification.DataAccess.Mappers
{
    internal interface ITriggerVariantMapper
    {
        TriggerVariantDto Map(TriggerVariant trigger);
    }

    internal class TriggerVariantMapper : ITriggerVariantMapper
    {
        private readonly IPolicyObjectsDeserializer _deserializer;

        public TriggerVariantMapper(IPolicyObjectsDeserializer deserializer)
        {
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public TriggerVariantDto Map(TriggerVariant trigger)
        {
            if (trigger == null)
                throw new ArgumentNullException(nameof(trigger));

            return new TriggerVariantDto
            {
                Id = trigger.Id,
                PolicyId = trigger.PolicyId,
                Name = trigger.Name,
                Iterative = trigger.Iterative,
                Preconditions = trigger.Preconditions,
                Conditions = trigger.Conditions,
                Commands = trigger.Commands.Select(Map).ToArray(),
                Schedule = trigger.Schedule == null ? null : Map(trigger.Schedule)
            };
        }

        private CommandConfig Map(Command command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (command.Config == null)
                throw new ArgumentNullException(nameof(command.Config));

            return command.Type switch
            {
                CommandType.AddTask           => _deserializer.Deserialize<AddTaskCommandConfig>(command.Config),
                CommandType.AddCollectionStep => _deserializer.Deserialize<AddCollectionStepCommandConfig>(command.Config),
                CommandType.SendTicket        => _deserializer.Deserialize<SendTickedCommandConfig>(command.Config),
                CommandType.AddCheck          => _deserializer.Deserialize<InstructCheckCommandConfig>(command.Config),
                _                             => throw new ArgumentOutOfRangeException()
            };
        }

        private Schedule Map(Entities.Policy.Schedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));            
            
            if (schedule.Value == null)
                throw new ArgumentNullException(nameof(schedule.Value));

            Schedule result = schedule.Type switch
            {
                ScheduleType.Cron     => _deserializer.Deserialize<CronSchedule>(schedule.Value),
                ScheduleType.Interval => _deserializer.Deserialize<IntervalSchedule>(schedule.Value),
                _                     => throw new ArgumentOutOfRangeException()
            };

            result.Offset = schedule.Offset;

            return result;
        }
    }
}