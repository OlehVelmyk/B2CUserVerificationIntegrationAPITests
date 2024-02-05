using System;

namespace WX.B2C.User.Verification.Automation.Services
{
    public struct SkippedCheck<T>
    {
        public T Check { get; private set; }

        public string Reason { get; private set; }

        public static SkippedCheck<T> Create(T check, string reason)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));
            if (reason == null)
                throw new ArgumentNullException(nameof(reason));

            return new SkippedCheck<T> { Check = check, Reason = reason };
        }
    }
}