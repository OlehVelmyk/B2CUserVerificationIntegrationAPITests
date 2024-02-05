using FluentValidation;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Validators
{
    public class CheckRequestValidator : BaseRequestValidator<CheckRequest>
    {
        public CheckRequestValidator()
        {
            RuleFor(request => request.VariantId).NotEmpty();
            RuleFor(request => request.Reason).NotEmpty();
        }
    }
}
