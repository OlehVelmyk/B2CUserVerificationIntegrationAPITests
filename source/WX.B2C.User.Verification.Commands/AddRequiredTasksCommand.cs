using System;

namespace WX.B2C.User.Verification.Commands
{
    public class AddRequiredTasksCommand : VerificationCommand
    {
        public AddRequiredTasksCommand(Guid userId,
                                       Guid applicationId, 
                                       Guid[] taskIds, 
                                       string reason)
        {
            CommandId = Guid.NewGuid();
            CommandChainId = userId;
            ApplicationId = applicationId;
            TaskIds = taskIds;
            Reason = reason;
        }

        public Guid ApplicationId { get; set; }

        public Guid[] TaskIds { get; set; }

        public string Reason { get; set; }
    }
}
