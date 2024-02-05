using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Models
{
    internal class Application
    {
        public Guid Id { get; set; }
        
        public ApplicationState State { get; set; }
    }
}