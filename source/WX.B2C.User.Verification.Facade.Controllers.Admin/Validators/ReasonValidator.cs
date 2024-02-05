using FluentValidation;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Validators
{
    public class ReasonValidator : BaseRequestValidator<ReasonDto>
    {
        public ReasonValidator()
        {
            RuleFor(dto => dto.Reason)
                .NotEmpty()
                .MaximumLength(2000);
        }
    }
}
