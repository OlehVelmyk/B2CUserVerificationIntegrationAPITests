using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Core.Contracts.Configurations.Options
{
    public class UserReminderOption : Option
    {
        public ReminderSpanOption[] Spans { get; }

        public UserReminderOption(ReminderSpanOption[] spans)
        {
            if (spans is null)
                throw new ArgumentNullException(nameof(spans));
            if (spans.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(spans));

            Spans = spans;
        }
    }

    public class ReminderSpanOption
    {
        public int Value { get; }

        public IntervalUnit Unit { get; }

        public ReminderSpanOption(int value, IntervalUnit unit)
        {
            Value = value;
            Unit = unit;
        }
    }
}
