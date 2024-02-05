using System;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos.Profile
{
    public sealed class PersonalDetailsDto
    {
        public Guid UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public AddressDto ResidenceAddress { get; set; }

        public string Nationality { get; set; }

        public string Email { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}