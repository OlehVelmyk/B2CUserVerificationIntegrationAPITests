using System;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class ResidenceAddress : AuditableEntity
    {
        public Guid UserId { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }
    }
}