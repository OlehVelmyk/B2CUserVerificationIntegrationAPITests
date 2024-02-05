using FluentValidation;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Validators
{
    public class CreateNoteRequestValidator : BaseRequestValidator<CreateNoteRequest>
    {
        public CreateNoteRequestValidator()
        {
            RuleFor(request => request.Subject).IsInEnum();
            RuleFor(request => request.SubjectId).NotEmpty();
            RuleFor(request => request.Text)
                .NotEmpty()
                .MaximumLength(2000);
        }
    }
}
