using System.Collections.Generic;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class CheckExternalDataDto : Dictionary<string, object>
    {
        public CheckExternalDataDto()
        {
        }

        public CheckExternalDataDto(IReadOnlyDictionary<string, object> data) : base(data)
        {
        }
    }
}