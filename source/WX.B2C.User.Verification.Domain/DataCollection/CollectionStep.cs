using System;
using WX.B2C.User.Verification.Domain.Exceptions;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.DataCollection
{
    /// <summary>
    /// Describe data which is required, but missing on the profile.
    /// It is typically generated when a check is instructed (but not enough data is available to perform it),
    /// or when a compliance officer explicitly requests further information
    /// or it can be generated automatically according to verification policy. 
    /// </summary>
    public class CollectionStep : AggregateRoot
    {
        public CollectionStep(Guid id,
                              Guid userId,
                              string xPath,
                              CollectionStepState collectionStepState,
                              bool isRequired,
                              bool isReviewNeeded,
                              CollectionStepReviewResult? reviewResult = null)
            : base(id)
        {
            UserId = userId;
            XPath = xPath;
            State = collectionStepState;
            IsRequired = isRequired;
            IsReviewNeeded = isReviewNeeded;
            ReviewResult = reviewResult;
        }

        public Guid UserId { get; }

        /// <summary>
        /// Specifies the data to be collected
        /// </summary>
        public string XPath { get; }

        public bool IsRequired { get; private set; }

        public bool IsReviewNeeded { get; private set; }

        public CollectionStepState State { get; private set; }
        
        public CollectionStepReviewResult? ReviewResult { get; private set; }

        /// <summary>
        /// Creates new collection step for user.
        /// </summary>
        public static CollectionStep Request(Guid id,
                                             Guid userId,
                                             string xPath,
                                             bool isRequired,
                                             bool isReviewNeeded,
                                             Initiation initiation)
        {
            var collectionStep = new CollectionStep(
                id, userId, xPath,
                CollectionStepState.Requested, isRequired, isReviewNeeded);

            var @event = CollectionStepRequested.Create(collectionStep, initiation);
            collectionStep.Apply(@event);

            return collectionStep;
        }

        public void Require(Initiation initiation)
        {
            if (IsRequired)
                return;

            if (State == CollectionStepState.Completed)
                throw new CollectionStepAlreadyCompletedException(Id, XPath);

            IsRequired = true;
            Apply(CollectionStepRequired.Create(this, initiation));
        }

        public void RequireReview(Initiation initiation)
        {
            if (IsReviewNeeded)
                return;

            if (State == CollectionStepState.Completed)
                throw new CollectionStepAlreadyCompletedException(Id, XPath);

            IsReviewNeeded = true;
            Apply(CollectionStepReviewRequested.Create(this, initiation));
        }

        public void MoveIntoReview(Initiation initiation)
        {
            if (State == CollectionStepState.InReview)
                return;

            if (!IsReviewNeeded)
                throw new CollectionStepReviewProhibitedException(Id, XPath);

            if (State != CollectionStepState.Requested)
                throw InvalidStateTransitionException.For<CollectionStep>(State, CollectionStepState.InReview);

            State = CollectionStepState.InReview;
            Apply(CollectionStepReadyForReview.Create(this, initiation));
        }

        public void Complete(Initiation initiation)
        {
            if (State == CollectionStepState.Completed)
                return;

            if (IsReviewNeeded && State == CollectionStepState.Requested)
                throw new CollectionStepReviewRequiredException(Id, XPath);

            State = CollectionStepState.Completed;
            Apply(CollectionStepCompleted.Create(this, initiation));
        }

        public void Review(CollectionStepReviewResult reviewResult, Initiation initiation)
        {
            if (!IsReviewNeeded)
                throw new CollectionStepReviewProhibitedException(Id, XPath);
            
            if (State != CollectionStepState.InReview)
                throw InvalidStateTransitionException.For<CollectionStep>(State, CollectionStepState.Completed);

            ReviewResult = reviewResult;
            Complete(initiation);
        }

        public void Cancel(Initiation initiation)
        {
            if (State == CollectionStepState.Cancelled)
                return;

            if (State != CollectionStepState.Requested)
                throw InvalidStateTransitionException.For<CollectionStep>(State, CollectionStepState.InReview);

            State = CollectionStepState.Cancelled;
            Apply(CollectionStepCancelled.Create(this, initiation));
        }

        public void Update(bool? isRequired, bool? isReviewNeeded, Initiation initiation)
        {
            if (State != CollectionStepState.Requested)
                throw InvalidStateException.Create<CollectionStep>(Id, CollectionStepState.Requested);

            IsRequired = isRequired ?? IsRequired;
            IsReviewNeeded = isReviewNeeded ?? IsReviewNeeded;
            Apply(CollectionStepUpdated.Create(this, initiation));
        }
    }
}
