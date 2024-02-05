using System;
using WX.B2C.User.Verification.Core.Contracts.Conditions;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;

namespace WX.B2C.User.Verification.DataAccess.Entities.Policy
{
    internal class TriggerVariant
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Can be onboarding or monitoring policy
        /// </summary>
        public Guid PolicyId { get; set; }

        public string Name { get; set; }

        public bool Iterative { get; set; }

        public Schedule Schedule { get; set; }

        public Condition[] Preconditions { get; set; }

        public Condition[] Conditions { get; set; }

        public Command[] Commands { get; set; }
    }

    internal class Command
    {
        public CommandType Type { get; set; }

        public string Config { get; set; }
    }

    public class Schedule
    {
        public ScheduleType Type { get; set; }

        public TimeSpan? Offset { get; set; }

        public string Value { get; set; }
    }

    public enum ScheduleType
    {
        Cron = 1,
        Interval = 2,
    }
}