using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Policy
{
    public class NewTaskDto
    {
        public Guid? Id { get; set; }

        public TaskType Type { get; set; }
        
        public Guid VariantId { get; set; }

        public Guid UserId { get; set; }

        public Guid[] AcceptanceCheckIds { get; set; }

        public Guid[] CollectionStepIds { get; set; }
    }
}