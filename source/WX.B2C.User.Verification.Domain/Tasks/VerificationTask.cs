using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Events;
using WX.B2C.User.Verification.Domain.Exceptions;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Models
{
    public class VerificationTask : AggregateRoot
    {
        private readonly HashSet<TaskCheck> _checks;
        private readonly HashSet<TaskCollectionStep> _collectionSteps;

        public VerificationTask(Guid id,
                                Guid userId,
                                Guid variantId,
                                TaskType taskType,
                                DateTime creationDate,
                                TaskState state,
                                TaskResult? taskResult,
                                TaskCollectionStep[] collectionSteps,
                                TaskCheck[] checks,
                                TaskExpirationDetails expirationDetails)
            : base(id)
        {
            VariantId = variantId;
            Type = taskType;
            CreationDate = creationDate;
            State = state;
            UserId = userId;

            if (State == TaskState.Completed && !taskResult.HasValue)
                throw new ArgumentNullException(nameof(taskResult));

            Result = taskResult;

            _checks = checks?.ToHashSet() ?? new HashSet<TaskCheck>();
            _collectionSteps = collectionSteps?.ToHashSet() ?? new HashSet<TaskCollectionStep>();
            ExpirationDetails = expirationDetails;
        }

        private VerificationTask(Guid id,
                                 Guid userId,
                                 Guid variantId,
                                 TaskType taskType,
                                 DateTime creationDate,
                                 TaskCollectionStep[] collectionSteps)
            : this(id,
                   userId,
                   variantId,
                   taskType,
                   creationDate,
                   TaskState.Incomplete,
                   null,
                   collectionSteps,
                   Array.Empty<TaskCheck>(),
                   null)
        {
        }

        public Guid UserId { get; }

        public TaskType Type { get; }

        public Guid VariantId { get; }

        public TaskState State { get; private set; }

        public TaskResult? Result { get; private set; }

        public bool IsExpired { get; private set; }

        public TaskExpirationDetails ExpirationDetails { get; private set; }

        public IReadOnlyCollection<TaskCheck> Checks => _checks;

        /// <summary>
        /// Collection step ids associated with task.
        /// </summary>
        public IReadOnlyCollection<TaskCollectionStep> CollectionSteps => _collectionSteps;

        public DateTime CreationDate { get; }

        public bool IsPassed => State == TaskState.Completed && Result == TaskResult.Passed;

        public static VerificationTask Create(Guid id,
                                              Guid userId,
                                              TaskType taskType,
                                              Guid variantId,
                                              DateTime creationDate,
                                              CollectionStep[] collectionSteps,
                                              Initiation initiation)
        {
            var taskCollectionSteps = collectionSteps.Select(TaskCollectionStep.Create).ToArray();

            var verificationTask = new VerificationTask(id, userId, variantId, taskType, creationDate, taskCollectionSteps);
            verificationTask.Apply(TaskCreated.Create(verificationTask, initiation));
            return verificationTask;
        }

        public void Complete(TaskResult result, Initiation initiation)
        {
            if (State == TaskState.Completed && Result == result)
                return;

            if (State == TaskState.Completed)
                throw new TaskAlreadyCompletedException(Id, Result.Value);

            if (result is TaskResult.Passed)
            {
                if (!AreStepsCompleted(out var notCompletedSteps))
                    throw new TaskStepsNotCompletedException(Id, notCompletedSteps);

                if (!AreChecksCompleted(out var notCompletedChecks))
                    throw new TaskChecksNotCompletedException(Id, notCompletedChecks);
            }

            State = TaskState.Completed;
            Result = result;

            Apply(TaskCompleted.Create(this, Result.Value, initiation));
        }

        public void Incomplete(Initiation initiation)
        {
            if (State == TaskState.Incomplete)
                return;

            var previousResult = Result;

            State = TaskState.Incomplete;
            Result = null;

            Apply(TaskIncomplete.Create(this, previousResult, initiation));
        }

        public void AddCollectionStep(CollectionStep collectionStep, Initiation initiation)
        {
            if (collectionStep == null)
                throw new ArgumentNullException(nameof(collectionStep));

            if (collectionStep.UserId != UserId)
                throw new UserMismatchedException(this, collectionStep);

            if (!_collectionSteps.Add(TaskCollectionStep.Create(collectionStep)))
                return;

            Apply(TaskCollectionStepAdded.Create(Id, UserId, collectionStep.Id, collectionStep.IsRequired, initiation));

            if (collectionStep.IsRequired && State is TaskState.Completed)
                Incomplete(initiation);
        }

        public void RemoveCollectionStep(Guid collectionStepId)
        {
            var collectionStep = CollectionSteps.Single(x => x.Id == collectionStepId);
            if (_collectionSteps.Remove(collectionStep))
                Apply(TaskCollectionStepRemoved.Create(Id, UserId, _collectionSteps.Select(step => step.Id).ToArray()));
        }

        private bool AreStepsCompleted(out IEnumerable<Guid> notCompletedSteps)
        {
            notCompletedSteps = CollectionSteps.Where(x => x.IsRequired && !x.IsCompleted).Select(step => step.Id);
            return !notCompletedSteps.Any();
        }

        private bool AreChecksCompleted(out IEnumerable<Guid> notCompletedChecks)
        {
            notCompletedChecks = Checks.Where(check => !IsCompletedCheck(check)).Select(x => x.Id);
            return !notCompletedChecks.Any();

            static bool IsCompletedCheck(TaskCheck check) =>
                check is { State: CheckState.Complete or CheckState.Error };
        }
    }
}