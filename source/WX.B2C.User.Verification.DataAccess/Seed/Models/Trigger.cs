using System;
using WX.B2C.User.Verification.Core.Contracts.Conditions;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.DataAccess.Entities.Policy;

namespace WX.B2C.User.Verification.DataAccess.Seed.Models
{
    internal class Trigger
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Iterative { get; set; }

        public Schedule Schedule { get; set; }

        public Condition[] Preconditions { get; set; }

        public Condition[] Conditions { get; set; }

        public Command[] Commands { get; set; }
    }

    internal class Schedule
    {
        public ScheduleType Type { get; set; }

        public TimeSpan? Offset { get; set; }

        public object Value { get; set; }
    }

    internal class Command
    {
        public CommandType Type { get; set; }

        public object Config { get; set; }
    }
}