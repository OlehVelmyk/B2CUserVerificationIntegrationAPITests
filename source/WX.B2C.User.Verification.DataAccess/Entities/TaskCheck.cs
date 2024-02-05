using System;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class TaskCheck
    {
        public Guid TaskId { get; set; }

        public Guid CheckId { get; set; }

        public virtual Check Check { get; set; }
    }
}