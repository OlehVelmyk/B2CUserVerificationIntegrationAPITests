using System;
using System.Linq;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class PriorityTaskDto
    {
        public Guid Id { get; set; }

        public Guid VariantId { get; set; }

        public TaskState State { get; set; }
    }

    public class TaskPriorityGroupDto
    {
        public int Priority { get; set; }

        public PriorityTaskDto[] Tasks { get; set; } = Array.Empty<PriorityTaskDto>();

        public Guid[] Checks { get; set; } = Array.Empty<Guid>();

        public bool IsCompleted => Tasks.All(task => task.State is TaskState.Completed);
    }
}
