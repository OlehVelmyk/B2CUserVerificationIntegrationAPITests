using System;
using WX.B2C.User.Verification.Domain.DataCollection;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps
{
    internal class NewCollectionStep
    {
        public static NewCollectionStep ReviewCompleted(CollectionStepReviewResult result) =>
            new()
            {
                State = CollectionStepState.Completed,
                IsRequired = true,
                Result = result
            };

        public static NewCollectionStep Completed() =>
            new()
            {
                State = CollectionStepState.Completed,
                IsRequired = true,
                Result = null
            };

        public static NewCollectionStep Incompleted(CollectionStepState state, bool isRequired = true)
        {
            if (state == CollectionStepState.Completed)
                throw new ArgumentException("Incompleted step must not have state {CollectionStepState.Completed}", nameof(state));
            
            return new()
            {
                State = state,
                IsRequired = isRequired,
                Result = null
            };
        }

        public CollectionStepState State { get; set; }
        public CollectionStepReviewResult? Result { get; set; }
        public bool IsRequired { get; set; }
    }
}