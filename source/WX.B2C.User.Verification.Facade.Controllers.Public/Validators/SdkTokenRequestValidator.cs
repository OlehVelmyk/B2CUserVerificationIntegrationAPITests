using FluentValidation;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Facade.Controllers.Public.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Validators
{
    public class SdkTokenRequestValidator : BaseRequestValidator<SdkTokenRequest>
    {
        public SdkTokenRequestValidator()
        {
            RuleFor(request => request.Type).IsInEnum();
            RuleFor(request => request.ApplicationId)
                .NotEmpty()
                .When(request => request.Type == TokenType.Application);
        }
    }
}
