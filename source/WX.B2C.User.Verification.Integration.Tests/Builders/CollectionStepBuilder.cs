using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Integration.Tests.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Builders
{
    internal class CollectionStepBuilder
    {
        private CollectionStep _result;

        public CollectionStepBuilder From(CollectionStepSpecimen specimen)
        {
            _result = new CollectionStep(
                specimen.Id,
                specimen.UserId,
                specimen.XPath,
                specimen.State,
                specimen.IsRequired,
                specimen.IsReviewNeeded);

            return this;
        }

        public CollectionStep Build() =>
            _result;
    }
}
