using FluentValidation;
using WX.B2C.User.Verification.Facade.Controllers.Internal.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Internal.Validators
{
    public class FacialSimilarityCheckRequestValidator : BaseRequestValidator<FacialSimilarityCheckRequest>
    {
        public FacialSimilarityCheckRequestValidator()
        {
            RuleFor(request => request.Variant).IsInEnum();
        }
    }
}
