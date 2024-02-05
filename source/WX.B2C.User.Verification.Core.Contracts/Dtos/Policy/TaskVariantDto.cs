using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Policy
{
    public class TaskVariantDto
    {
        public Guid VariantId { get; set; }

        public string TaskName { get; set; }

        public TaskType Type { get; set; }

        public int Priority { get; set; }

        public PolicyCollectionStep[] CollectionSteps { get; set; }

        public Guid[] CheckVariants { get; set; }

        public AutoCompletePolicy AutoCompletePolicy { get; set; }
    }
}