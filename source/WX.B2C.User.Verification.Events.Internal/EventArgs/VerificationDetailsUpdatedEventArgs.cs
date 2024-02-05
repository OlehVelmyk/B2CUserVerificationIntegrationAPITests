using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class VerificationDetailsUpdatedEventArgs : System.EventArgs
    {
        public Guid UserId { get; set; }

        public PropertyChangeDto[] Changes { get; set; }

        public InitiationDto Initiation { get; set; }

        public static VerificationDetailsUpdatedEventArgs Create(Guid userId, PropertyChangeDto[] changes, InitiationDto initiation) =>
            new VerificationDetailsUpdatedEventArgs { UserId = userId, Changes = changes, Initiation = initiation };
    }
}
