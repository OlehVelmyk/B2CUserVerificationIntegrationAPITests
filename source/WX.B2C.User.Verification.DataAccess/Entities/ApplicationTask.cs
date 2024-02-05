using System;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class ApplicationTask
    {
        public Guid ApplicationId { get; set; }

        public Guid TaskId { get; set; }

        public virtual VerificationTask Task { get; set; }
    }
}