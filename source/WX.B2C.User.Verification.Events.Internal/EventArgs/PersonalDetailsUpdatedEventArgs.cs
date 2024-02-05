using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class PersonalDetailsUpdatedEventArgs : System.EventArgs
    {
        public Guid UserId { get; set; }

        public PropertyChangeDto[] Changes { get; set; }

        public InitiationDto Initiation { get; set; }

        public static PersonalDetailsUpdatedEventArgs Create(Guid userId, PropertyChangeDto[] changes, InitiationDto initiation) =>
            new PersonalDetailsUpdatedEventArgs { UserId = userId, Changes = changes, Initiation = initiation };
    }
}
