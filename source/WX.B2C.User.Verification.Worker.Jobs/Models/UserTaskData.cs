using System;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    public class UserTaskData : IJobData
    {
        public Guid UserId { get; set; }

        public TaskType TaskType { get; set; }

        public TaskStateInfo[] Tasks { get; set; }
    }

    public class TaskStateInfo
    {
        public Guid UserId { get; set; }

        public Guid Id { get; set; }

        public TaskState State { get; set; }

        public TaskResult? Result { get; set; }
    }
}