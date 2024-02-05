using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    internal class TasksCreatingData : IJobData
    {
        public Guid UserId { get; set; }

        public Guid ApplicationId { get; set; }

        public TaskVariantData[] TaskVariants { get; set; }

        internal class TaskVariantData
        {
            public Guid Id { get; set; }

            public TaskType Type { get; set; }

            public bool AlreadyCreated { get; set; }
        }
    }
}
