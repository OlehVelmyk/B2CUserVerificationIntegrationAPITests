using FluentValidation;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Validators
{
    public class CompleteTaskRequestValidator : BaseRequestValidator<CompleteTaskRequest>
    {
        public CompleteTaskRequestValidator()
        {
            RuleFor(request => request.Result).IsInEnum();
            RuleFor(request => request.Reason).NotEmpty();
        }
    }
}
