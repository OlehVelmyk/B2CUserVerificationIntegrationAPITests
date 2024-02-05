using FluentValidation;
using WX.B2C.User.Verification.Core.Contracts.Enum;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Validators
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
