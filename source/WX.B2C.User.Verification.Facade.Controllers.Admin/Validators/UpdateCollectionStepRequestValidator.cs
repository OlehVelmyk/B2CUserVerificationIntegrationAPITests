using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Requests;
using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Validation;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Validators
{
    public class UpdateCollectionStepRequestValidator : BaseRequestValidator<UpdateCollectionStepRequest>
    {
        public UpdateCollectionStepRequestValidator()
        {
            RuleFor(request => request).Custom(ValidateProperties);
            RuleFor(request => request.Reason).NotEmpty();
        }

        private static void ValidateProperties(UpdateCollectionStepRequest request, 
                                               ValidationContext<UpdateCollectionStepRequest> context)
        {
            var properties = new Dictionary<string, object>
            {
                [nameof(request.IsRequired)] = request.IsRequired,
                [nameof(request.IsReviewNeeded)] = request.IsReviewNeeded
            };

            var allPropertiesUndefined = properties.All(property => property.Value is null);
            if (allPropertiesUndefined)
                context.AddFailure(nameof(request), "At least one property should be defined.");
        }
    }
}
