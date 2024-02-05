using System;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class ApplicationStateChangelogDto
    {
        public Guid ApplicationId { get; set; }

        public DateTime? FirstApprovedDate { get; set; }

        public DateTime? LastApprovedDate { get; set; }
    }
}
