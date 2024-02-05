using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class ApplicationTaskDto
    {
        public Guid Id { get; set; }

        public Guid VariantId { get; set; }

        public TaskType Type { get; set; }

        public TaskState State { get; set; }

        public TaskResult? Result { get; set; }

        public bool IsExpired { get; set; }
    }
}
