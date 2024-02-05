using System;
using WX.B2C.User.Verification.Events.Dtos;

namespace WX.B2C.User.Verification.Events.EventArgs
{
    public class VerificationDetailsUpdatedEventArgs : System.EventArgs
    {
        public Guid UserId { get; set; }

        public VerificationDetailsDto VerificationDetails { get; set; }

        public string[] Changes { get; set; }
    }
}
