using System;

namespace WX.B2C.User.Verification.DataAccess.Entities
{
    internal class PersonalDetails : AuditableEntity
    {
        public Guid UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public ResidenceAddress ResidenceAddress { get; set; }

        public string Nationality { get; set; }

        public string Email { get; set; }
    }
}