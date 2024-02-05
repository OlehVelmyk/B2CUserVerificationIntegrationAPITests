using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers.Tasks
{
    internal class TaskVariant
    {
        public Guid Id { get; set; }

        public TaskType Type { get; set; }
    }
}
