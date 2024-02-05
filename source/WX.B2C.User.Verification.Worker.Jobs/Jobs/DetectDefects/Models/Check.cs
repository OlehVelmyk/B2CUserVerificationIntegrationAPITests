using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models
{
    internal class Check
    {
        public Guid Id { get; set; }
        
        public Guid[] RelatedTasks { get; set; } = Array.Empty<Guid>();

        public CheckState State { get; set; }

        public bool IsCompleted => State is CheckState.Complete or CheckState.Error;

        public override string ToString() =>
            Id.ToString();
    }
}