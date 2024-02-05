using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Triggers;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class Trigger
    {
        public Guid Id { get; set; }

        public Guid VariantId { get; set; }

        public Guid UserId { get; set; }

        public Guid ApplicationId { get; set; }

        public TriggerState State { get; set; }

        public DateTime ScheduleDate { get; set; }

        public DateTime? UnscheduleDate { get; set; }

        public DateTime? FiringDate { get; set; }

        public Dictionary<string, object> Context { get; set; }
    }
}