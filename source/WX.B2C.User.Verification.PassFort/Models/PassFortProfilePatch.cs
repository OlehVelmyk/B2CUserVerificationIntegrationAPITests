using System;
using Optional;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.PassFort.Models
{
    public sealed class PassFortProfilePatch
    {
        public Option<string> Email { get; set; }

        public Option<FullNameDto> FullName { get; set; }

        public Option<string> Nationality { get; set; }

        public Option<DateTime> BirthDate { get; set; }

        public Option<(string, IdDocumentNumberDto)> IdDocumentData { get; set; }

        public Option<AddressDto> Address { get; set; }
    }
}
