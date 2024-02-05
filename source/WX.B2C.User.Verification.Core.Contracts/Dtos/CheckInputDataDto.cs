using System.Collections.Generic;
using System.Runtime.Serialization;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    [KnownType(typeof(FullNameDto))]
    [KnownType(typeof(AddressDto))]
    [KnownType(typeof(DocumentDto))]
    [KnownType(typeof(string[]))]
    [KnownType(typeof(IdDocumentNumberDto))]
    [KnownType(typeof(TinDto))]
    public class CheckInputDataDto : Dictionary<string, object>
    {
        public CheckInputDataDto()
        {
        }

        public CheckInputDataDto(IReadOnlyDictionary<string, object> data) : base(data)
        {
        }
    }
}
