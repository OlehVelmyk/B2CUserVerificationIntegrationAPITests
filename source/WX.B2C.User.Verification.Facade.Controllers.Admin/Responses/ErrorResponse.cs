using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Responses
{
    public sealed class ErrorResponse
    {
        [JsonProperty(PropertyName = "errors")]
        public ErrorDetails[] Errors { get; set; }

        public static ErrorResponse Create(params ErrorDetails[] errorDetails)
        {
            if (errorDetails == null)
                throw new ArgumentNullException(nameof(errorDetails));

            return new ErrorResponse
            {
                Errors = errorDetails
            };
        }
    }
}
