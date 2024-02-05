using System;
using WX.B2C.User.Verification.Events.Internal.Dtos;

namespace WX.B2C.User.Verification.Events.Internal.EventArgs
{
    public class ApplicationRequiredTaskAddedEventArgs : System.EventArgs
    {
        public Guid ApplicationId { get; set; }
        
        public Guid UserId { get; set; }

        public Guid TaskId { get; set; }

        public InitiationDto Initiation { get; set; }

        public static ApplicationRequiredTaskAddedEventArgs Create(Guid applicationId, Guid userId, Guid taskId, InitiationDto initiation) =>
            new ApplicationRequiredTaskAddedEventArgs
            {
                ApplicationId = applicationId, 
                UserId = userId, 
                TaskId = taskId, 
                Initiation = initiation
            };
    }
}