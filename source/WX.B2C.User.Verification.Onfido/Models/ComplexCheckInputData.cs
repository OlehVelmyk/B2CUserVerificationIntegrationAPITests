using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.Onfido.Models
{
    internal class ComplexCheckInputData : OnfidoCheckInputData
    {
        public FullNameDto FullName { get; set; }

        public DocumentDto Selfie { get; set; }

        public DocumentDto IdentityDocument { get; set; }

        public DateTime? BirthDate { get; set; }

        public AddressDto Address { get; set; }
    }
}