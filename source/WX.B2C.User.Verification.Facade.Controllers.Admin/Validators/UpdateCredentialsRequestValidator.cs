using FluentValidation;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Validators
{
    public class UpdateCredentialsRequestValidator : BaseRequestValidator<UpdateCredentialsRequest>
    {
        public UpdateCredentialsRequestValidator()
        {
            RuleFor(request => request.UserId).NotEmpty();
            RuleFor(request => request.NewPassword).NotEmpty();
        }
    }
}
