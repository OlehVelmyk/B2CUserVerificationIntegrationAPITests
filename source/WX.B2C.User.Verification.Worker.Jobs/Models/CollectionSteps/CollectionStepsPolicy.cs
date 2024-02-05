using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps
{
    internal class CollectionStepsPolicy
    {
        public PolicyCollectionStep[] Steps { get; set; }

        public TaskVariant[] Tasks { get; set; }
    }
}