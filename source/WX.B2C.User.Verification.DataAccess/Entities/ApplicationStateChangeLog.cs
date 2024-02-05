using System;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class ApplicationStateChangelog
    {
        public Guid ApplicationId { get; set; }

        public DateTime? FirstApprovedDate { get; set; }

        public DateTime? LastApprovedDate { get; set; }

        public Application Application { get; set; }
    }
}