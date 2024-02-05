using System;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class ChecksCreatedEventArgs : System.EventArgs
    {
        public Guid UserId { get; set; }

        public Guid[] Checks { get; set; }
        
        public static ChecksCreatedEventArgs Create(Guid userId,
                                                    Guid[] checks) =>
            new ChecksCreatedEventArgs
            {
                UserId = userId,
                Checks = checks
            };
    }
}
