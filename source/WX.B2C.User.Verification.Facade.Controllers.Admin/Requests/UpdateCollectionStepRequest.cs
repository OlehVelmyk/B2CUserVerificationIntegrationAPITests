using WX.B2C.User.Verification.Infrastructure.ApiMiddleware.Swagger.Attributes;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Requests
{
    public class UpdateCollectionStepRequest
    {
        [NotRequired]
        public bool? IsReviewNeeded { get; set; }

        [NotRequired]
        public bool? IsRequired { get; set; }

        public string Reason { get; set; }
    }
}
