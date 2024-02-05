using System;
using WX.B2C.User.Verification.Events.Internal.Enums;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class CheckCompletedEventArgs : System.EventArgs
    {
        public Guid CheckId { get; set; }
        
        public Guid UserId { get; set;}
        
        public Guid VariantId { get; set; }
        
        public CheckType Type { get; set; }

        public CheckProviderType Provider { get; set; }

        public CheckResult Result { get; set; }

        public string Decision { get; set; }

        public static CheckCompletedEventArgs Create(Guid checkId,
                                                     Guid userId,
                                                     Guid variantId,
                                                     CheckType type,
                                                     CheckProviderType provider,
                                                     CheckResult result,
                                                     string decision) =>
            new CheckCompletedEventArgs
            {
                CheckId = checkId,
                UserId = userId,
                VariantId = variantId,
                Type = type,
                Provider = provider,
                Result = result,
                Decision = decision
            };
    }
}