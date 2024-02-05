using System;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Events
{
    public class PersonalDetailsUpdated : DomainEvent
    {
        public Guid UserId { get; private set; }

        public PropertyChange[] Changes { get; private set; }

        public Initiation Initiation { get; private set; }

        public static PersonalDetailsUpdated Create(Guid userId, PropertyChange[] changes, Initiation initiation) =>
            new() {UserId = userId, Changes = changes, Initiation = initiation};
    }
}
