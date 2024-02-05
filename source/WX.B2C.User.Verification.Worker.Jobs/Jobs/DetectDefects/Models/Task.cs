using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models
{
    public class Task
    {
        public Guid Id { get; set; }

        public TaskState State { get; set; }

        public TaskResult? Result { get; set; }

        public TaskType Type { get; set; }

        public Guid? ApplicationId { get; set; }
        
        public override string ToString() =>
            $"{Id}:{Type}";
    }
}