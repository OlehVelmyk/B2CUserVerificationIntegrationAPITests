using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain
{
    public class TaskExpirationDetails : ValueObject
    {
        public TaskExpirationDetails(TaskExpirationReason expirationReason, DateTime expiredAt)
        {
            if (!Enum.IsDefined(typeof(TaskExpirationReason), expirationReason))
                throw new ArgumentException($"Invalid value: {expirationReason}", nameof(expirationReason));
            if (expiredAt == default)
                throw new ArgumentException($"Invalid value: {expiredAt}", nameof(expiredAt));

            ExpirationReason = expirationReason;
            ExpiredAt = expiredAt;
        }

        public TaskExpirationReason ExpirationReason { get; }

        public DateTime ExpiredAt { get; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return ExpirationReason;
            yield return ExpiredAt;
        }
    }
}