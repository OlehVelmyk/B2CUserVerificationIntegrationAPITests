using System;

namespace WX.B2C.User.Verification.DataAccess.Entities.Policy
{
    internal class PolicyTask
    {
        public Guid PolicyId { get; set; }

        public Guid TaskVariantId { get; set; }

        public TaskVariant TaskVariant { get; set; }
    }
}