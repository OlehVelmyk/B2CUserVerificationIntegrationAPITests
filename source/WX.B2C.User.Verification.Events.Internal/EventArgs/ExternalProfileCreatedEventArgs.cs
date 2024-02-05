using System;
using WX.B2C.User.Verification.Events.Internal.Enums;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class ExternalProfileCreatedEventArgs : System.EventArgs
    {
        public Guid UserId { get; set; }

        public ExternalProviderType ProviderType { get; set; }

        public static ExternalProfileCreatedEventArgs Create(Guid userId, ExternalProviderType providerType) =>
            new ExternalProfileCreatedEventArgs { UserId = userId, ProviderType = providerType};
    }
}