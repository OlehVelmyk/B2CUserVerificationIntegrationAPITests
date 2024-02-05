using FluentValidation;
using FluentValidation.Results;

namespace WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation
{
    public abstract class BaseRequestValidator<TRequest> : AbstractValidator<TRequest>
    {
        protected override bool PreValidate(ValidationContext<TRequest> context, ValidationResult result)
        {
            if (context.InstanceToValidate is null)
                context.AddFailure("request", "Please ensure a request was supplied.");

            return result.IsValid;
        }
    }
}
