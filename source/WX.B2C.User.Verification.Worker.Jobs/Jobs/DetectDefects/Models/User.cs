using System;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models
{
    /// <summary>
    /// FIX invent normal name
    /// </summary>
    internal class UserConsistency : IJobData
    {
        public Guid UserId { get; set; }

        public ProfileDataExistence ProfileDataExistence { get; set; } = new();
        
        public Application Application { get; set; }
        
        public string PassFortProfileId { get; set; }
        
        public string OnfidoApplicationId { get; set; }

        public Region Region { get; set; }
        
        public Task[] Tasks { get; set; } = Array.Empty<Task>();
        
        public Check[] Checks { get; set; } = Array.Empty<Check>();
        
        public CollectionStep[] CollectionSteps { get; set; } = Array.Empty<CollectionStep>();
    }
}