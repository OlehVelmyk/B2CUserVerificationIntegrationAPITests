using WX.B2C.User.Verification.Domain.DataCollection;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Requests
{
    public class ReviewCollectionStepRequest
    {
        public CollectionStepReviewResult ReviewResult { get; set; }

        public string Reason { get; set; }
    }
}