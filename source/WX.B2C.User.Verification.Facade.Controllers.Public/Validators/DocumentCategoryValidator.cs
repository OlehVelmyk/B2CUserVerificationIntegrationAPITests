using FluentValidation;
using WX.B2C.User.Verification.Domain.Enums;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Validators
{
    public class DocumentCategoryValidator : AbstractValidator<DocumentCategory>
    {
        public DocumentCategoryValidator()
        {
            RuleFor(category => category).IsInEnum();
        }
    }
}