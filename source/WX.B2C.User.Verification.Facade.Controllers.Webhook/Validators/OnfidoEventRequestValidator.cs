using FluentValidation;
using WX.B2C.User.Verification.Facade.Controllers.Webhook.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Webhook.Validators
{
    public class OnfidoEventRequestValidator : BaseRequestValidator<OnfidoEventRequest>
    {
        public OnfidoEventRequestValidator()
        {
            RuleFor(x => x.Payload)
                .NotNull();

            RuleFor(x => x.Payload.ResourceType)
                .NotEmpty();

            RuleFor(x => x.Payload.Object)
                .NotNull();

            RuleFor(x => x.Payload.Object.Id)
                .NotEmpty();

            RuleFor(x => x.Payload.Object.Status)
                .NotEmpty();

            RuleFor(x => x.Payload.Object.Href)
                .NotEmpty();
        }
    }
}
