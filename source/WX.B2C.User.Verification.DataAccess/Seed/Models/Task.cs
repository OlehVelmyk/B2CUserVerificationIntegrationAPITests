using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.DataAccess.Seed.Models
{
    internal class Task
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public TaskType Type { get; set; }

        public int Priority { get; set; }

        public PolicyCollectionStep[] CollectionSteps { get; set; }

        public Guid[] ChecksVariants { get; set; }

        public AutoCompletePolicy AutoCompletePolicy { get; set; }
    }
}