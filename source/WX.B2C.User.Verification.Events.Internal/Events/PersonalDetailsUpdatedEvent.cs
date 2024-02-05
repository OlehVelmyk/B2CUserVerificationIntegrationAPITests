using System;
using WX.B2C.User.Verification.Events.Internal.EventArgs;

namespace WX.B2C.User.Verification.Events.Internal.Events
{
    public class PersonalDetailsUpdatedEvent : BaseEvent<PersonalDetailsUpdatedEventArgs>
    {
        public PersonalDetailsUpdatedEvent(string key,
                                           PersonalDetailsUpdatedEventArgs args,
                                           Guid causationId,
                                           Guid correlationId)
            :base(key,args,causationId,correlationId) { }
    }
}