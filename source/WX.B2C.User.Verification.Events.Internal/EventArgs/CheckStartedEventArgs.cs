using System;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class CheckStartedEventArgs : System.EventArgs
    {
        public Guid CheckId { get; set; }

        public Guid UserId { get; set; }

        public Guid VariantId { get; set; }

        public static CheckStartedEventArgs Create(Guid checkId, Guid userId, Guid variantId)
        {
            return new CheckStartedEventArgs
            {
                UserId = userId,
                CheckId = checkId,
                VariantId = variantId
            };
        }
    }
}