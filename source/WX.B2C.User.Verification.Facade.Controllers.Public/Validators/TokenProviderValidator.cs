using FluentValidation;
using WX.B2C.User.Verification.Core.Contracts.Enum;

namespace WX.B2C.User.Verification.Facade.Controllers.Public.Validators
{
    public class TokenProviderValidator : AbstractValidator<TokenProvider>
    {
        public TokenProviderValidator()
        {
            RuleFor(provider => provider).IsInEnum();
        }
    }
}
