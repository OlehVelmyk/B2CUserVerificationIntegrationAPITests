using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    internal class UserInstructChecksData : IJobData
    {
        public Guid UserId { get; set; }

        public TaskAcceptanceChecksData[] TaskAcceptanceChecks { get; set; }

        public Guid? ApplicationId { get; set; }
    }

    internal class TaskAcceptanceChecksData
    {
        public TaskType TaskType { get; set; }

        public Guid[] NewChecks { get; set; }
        
        public TaskData[] Tasks { get; set; }

        internal class TaskData
        {
            public Guid Id { get; set; }

            public Guid[] ExitingChecks { get; set; }
        }
    }
}
