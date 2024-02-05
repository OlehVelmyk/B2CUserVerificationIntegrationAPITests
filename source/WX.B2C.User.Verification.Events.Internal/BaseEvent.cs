using System;
using WX.Messaging.Core;

namespace WX.B2C.User.Verification.Events.Internal
{
    public class BaseEvent<TEventArgs> : Event<TEventArgs> where TEventArgs : System.EventArgs
    {
        protected BaseEvent(string key,
                            TEventArgs eventArgs,
                            Guid causationId,
                            Guid? correlationId = null)
            : base(key, eventArgs, causationId, correlationId) { }

        public override string ChannelName => ChannelNames.ChannelName;
    }
}