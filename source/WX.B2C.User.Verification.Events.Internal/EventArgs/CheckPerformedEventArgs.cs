using System;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class CheckPerformedEventArgs : System.EventArgs
    {
        public Guid CheckId { get; set; }

        public Guid UserId { get; set; }

        public Guid VariantId { get; set; }

        public static CheckPerformedEventArgs Create(Guid checkId, Guid userId, Guid variantId)
        {
            return new CheckPerformedEventArgs
            {
                UserId = userId,
                CheckId = checkId,
                VariantId = variantId
            };
        }
    }
}
