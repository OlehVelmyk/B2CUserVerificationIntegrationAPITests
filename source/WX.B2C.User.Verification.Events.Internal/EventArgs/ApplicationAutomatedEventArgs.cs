using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class ApplicationAutomatedEventArgs : System.EventArgs
    {
        public Guid UserId { get; set; }

        public Guid ApplicationId { get; set; }

        public InitiationDto Initiation { get; set; }
    }
}