using FluentValidation;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Validators
{
    public class ReviewCollectionStepRequestValidator: BaseRequestValidator<ReviewCollectionStepRequest>
    {
        public ReviewCollectionStepRequestValidator()
        {
            RuleFor(request => request.ReviewResult).IsInEnum();
        }
    }
}