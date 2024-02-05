﻿using System;
using WX.B2C.User.Verification.Events.EventArgs;

namespace WX.B2C.User.Verification.Events.Events
{
    public class CollectionStepRequestedEvent : BaseEvent<CollectionStepRequestedEventArgs>
    {
        public CollectionStepRequestedEvent(string key,
                                           CollectionStepRequestedEventArgs args,
                                           Guid causationId,
                                           Guid correlationId)
            : base(key, args, causationId, correlationId) { }
    }
}