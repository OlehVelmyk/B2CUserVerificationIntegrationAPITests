using FluentValidation;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Validators
{
    public class UpdateVerificationDetailsRequestValidator : BaseRequestValidator<UpdateVerificationDetailsRequest>
    {
        public UpdateVerificationDetailsRequestValidator()
        {
            RuleFor(request => request.Reason).NotEmpty();

            RuleFor(request => request.TaxResidence)
                .OnlyUniqueValues()
                .When(request => request.TaxResidence != null);
        }
    }
}
