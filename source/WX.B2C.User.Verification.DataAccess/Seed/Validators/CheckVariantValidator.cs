using FluentValidation;
using WX.B2C.User.Verification.DataAccess.Seed.Models;

namespace WX.B2C.User.Verification.DataAccess.Seed.Validators
{
    internal class CheckVariantValidator : BaseSeedValidator<CheckVariant>
    {
        public CheckVariantValidator()
        {
            RuleFor(check => check.Id).NotEmpty();
            RuleFor(check => check.Name).NotEmpty();
            RuleFor(check => check.Type).IsInEnum();
            RuleFor(check => check.Provider).IsInEnum();
        }
    }
}