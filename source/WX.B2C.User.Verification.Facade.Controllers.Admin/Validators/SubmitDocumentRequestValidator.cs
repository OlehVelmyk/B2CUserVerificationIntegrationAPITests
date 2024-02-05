using FluentValidation;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Validators
{
    public class SubmitDocumentRequestValidator : BaseRequestValidator<SubmitDocumentRequest>
    {
        public SubmitDocumentRequestValidator()
        {
            RuleFor(request => request.Category).IsInEnum();
            RuleFor(request => request.Type).NotEmpty();
            RuleFor(request => request.Files).NotEmpty();
            RuleFor(request => request.Reason).NotEmpty();
        }
    }
}
