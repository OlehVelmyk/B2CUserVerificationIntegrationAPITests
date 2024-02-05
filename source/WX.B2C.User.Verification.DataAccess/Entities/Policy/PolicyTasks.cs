using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.DataAccess.Entities.Policy
{
    internal class TaskVariant
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public TaskType Type { get; set; }

        public int Priority { get; set; }

        public PolicyCollectionStep[] CollectionSteps { get; set; }

        public AutoCompletePolicy AutoCompletePolicy { get; set; }

        public HashSet<TaskCheckVariant> ChecksVariants { get; set; }
    }
}