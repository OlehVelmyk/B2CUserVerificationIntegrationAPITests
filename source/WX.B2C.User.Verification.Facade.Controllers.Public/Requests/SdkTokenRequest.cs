using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Requests
{
    public class SdkTokenRequest
    {
        public TokenType Type { get; set; }

        [NotRequired]
        public string ApplicationId { get; set; }
    }
}
