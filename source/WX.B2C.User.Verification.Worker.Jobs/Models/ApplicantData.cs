using System;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    internal class ApplicantData : IJobData
    {
        public string Id { get; set; }

        public Guid UserId { get; set; }
    }
}
