using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Provider.Contracts.Models;

namespace WX.B2C.User.Verification.Onfido.Models
{
    internal class IdentityEnhancedCheckInputData : OnfidoCheckInputData
    {
        public FullNameDto FullName { get; set; }

        public DateTime BirthDate { get; set; }

        public AddressDto Address { get; set; }
    }

    internal class IdentityEnhancedCheckOutputData : CheckOutputData
    { }
}