using System;
using WX.B2C.User.Verification.Events.Internal.Enums;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class CheckErrorOccuredEventArgs : System.EventArgs
    {
        public Guid CheckId { get; set; }

        public Guid UserId { get; set; }

        public Guid VariantId { get; set; }

        public CheckType Type { get; set; }

        public static CheckErrorOccuredEventArgs Create(Guid checkId,
                                                     Guid userId,
                                                     Guid variantId,
                                                     CheckType type) =>
            new CheckErrorOccuredEventArgs
            {
                CheckId = checkId,
                UserId = userId,
                VariantId = variantId,
                Type = type
            };
    }
}
