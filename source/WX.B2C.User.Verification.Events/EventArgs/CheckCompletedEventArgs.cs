using System;
using WX.B2C.User.Verification.Events.Enums;

namespace WX.B2C.User.Verification.Events.EventArgs
{
    public class CheckCompletedEventArgs : System.EventArgs
    {
        public Guid UserId { get; set; }

        public Guid CheckId { get; set; }

        public Guid VariantId { get; set; }

        public CheckType Type { get; set; }

        public CheckResult Result { get; set; }

        public static CheckCompletedEventArgs Create(Guid userId, Guid checkId, Guid variantId, CheckType type, CheckResult result) =>
            new CheckCompletedEventArgs
            {
                UserId = userId,
                CheckId = checkId,
                VariantId = variantId,
                Type = type,
                Result = result
            };
    }
}
