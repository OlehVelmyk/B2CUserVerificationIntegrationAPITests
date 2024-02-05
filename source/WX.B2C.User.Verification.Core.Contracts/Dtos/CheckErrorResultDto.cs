using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class CheckErrorResultDto
    {
        public string RawData { get; set; }

        public CheckErrorDto[] Errors { get; set; }
    }

    public class CheckErrorDto
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public ErrorAdditionalDataDto AdditionalData { get; set; }
    }

    [KnownType(typeof(string[]))]
    public class ErrorAdditionalDataDto : Dictionary<string, object>
    {
        public ErrorAdditionalDataDto()
        {
        }

        public ErrorAdditionalDataDto(IDictionary<string, object> additionalData)
            : base(additionalData)
        {
        }
    }
}
