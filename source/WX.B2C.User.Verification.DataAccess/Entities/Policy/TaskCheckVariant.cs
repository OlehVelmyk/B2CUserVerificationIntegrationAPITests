using System;

namespace WX.B2C.User.Verification.DataAccess.Entities.Policy
{
    internal class TaskCheckVariant
    {
        public Guid TaskId { get; set; }
        
        public Guid CheckVariantId { get; set; }

        public virtual PolicyCheckVariant CheckVariant { get; set; }
    }
}