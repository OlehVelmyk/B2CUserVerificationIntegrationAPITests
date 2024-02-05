using System;
using WX.B2C.User.Verification.Worker.Jobs.Services;

namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    internal class ApplicationData : IJobData
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid PolicyId { get; set; }
    }
}
