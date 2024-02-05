using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps
{
    internal class TaskVariant
    {
        public Guid VariantId { get; set; }

        public TaskType Type { get; set; }

        public string[] XPathes { get; set; }
    }
}