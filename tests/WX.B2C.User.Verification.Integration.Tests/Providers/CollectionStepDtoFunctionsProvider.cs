using WX.B2C.User.Verification.Api.Admin.Client.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Providers
{
    internal class CollectionStepDtoFunctionsProvider
    {
        public static bool PoANotRequiredRequested(CollectionStepDto step) =>
            !step.IsRequired &&
            step.IsReviewNeeded &&
            step.State is CollectionStepState.Requested &&
            step.Variant is DocumentCollectionStepVariantDto { DocumentCategory: DocumentCategory.ProofOfAddress };

        public static bool PoANotRequiredInReview(CollectionStepDto step) =>
            !step.IsRequired &&
            step.IsReviewNeeded &&
            step.State is CollectionStepState.InReview &&
            step.Variant is DocumentCollectionStepVariantDto { DocumentCategory: DocumentCategory.ProofOfAddress };

        public static bool PoARequired(CollectionStepDto step) =>
            step.IsRequired &&
            step.IsReviewNeeded &&
            step.State is CollectionStepState.Requested &&
            step.Variant is DocumentCollectionStepVariantDto { DocumentCategory: DocumentCategory.ProofOfAddress };
    }
}
