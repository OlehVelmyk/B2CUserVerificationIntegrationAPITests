using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.ProviderModels
{
    internal class Check
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? TaskId { get; set; }

        public CheckState State { get; set; }
    }
}