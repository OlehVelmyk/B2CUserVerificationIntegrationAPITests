using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Domain.Events;
using WX.B2C.User.Verification.Domain.Exceptions;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Models
{
    public class Check : AggregateRoot
    {
        public Check(Guid id,
                     Guid userId,
                     CheckType type,
                     CheckVariant variant,
                     CheckState state,
                     CheckExecutionContext executionContext = null,
                     CheckProcessingResult processingResult = null,
                     CheckError[] errors = null,
                     DateTime? startedAt = null,
                     DateTime? performedAt = null,
                     DateTime? completedAt = null,
                     Guid[] relatedTasks = null) 
            : base(id)
        {
            UserId = userId;
            Type = type;
            Variant = variant;
            State = state;
            RelatedTasks = relatedTasks ?? Array.Empty<Guid>();
            ExecutionContext = executionContext;
            ProcessingResult = processingResult;
            Errors = errors;
            StartedAt = startedAt;
            PerformedAt = performedAt;
            CompletedAt = completedAt;
        }

        public Guid UserId { get; }

        public CheckType Type { get; }

        public CheckVariant Variant { get; }

        public CheckState State { get; private set; }

        public CheckExecutionContext ExecutionContext { get; private set; }

        public CheckProcessingResult ProcessingResult { get; private set; }

        public ICollection<CheckError> Errors { get; private set; }

        public DateTime? StartedAt { get; private set; }

        public DateTime? PerformedAt { get; private set; }

        public DateTime? CompletedAt { get; private set; }

        public Guid[] RelatedTasks { get; private set; }

        public bool IsCompleted => State is CheckState.Complete or CheckState.Error;

        public static Check Create(Guid checkId,
                                   Guid userId,
                                   CheckType checkType,
                                   CheckVariant variant,
                                   Guid[] relatedTasks,
                                   Initiation initiation)
        {
            const CheckState pendingState = CheckState.Pending;
            var check = new Check(
                checkId,
                userId,
                checkType,
                variant,
                pendingState,
                relatedTasks: relatedTasks);

            check.Apply(CheckCreated.Create(check, initiation));
            return check;
        }

        public void Start(CheckExecutionContext executionContext, DateTime startedAt)
        {
            ChangeState(CheckState.Running);
            ExecutionContext = executionContext;
            StartedAt = startedAt;

            var @event = CheckStarted.Create(this);
            Apply(@event);
        }

        public void Performed(DateTime performedAt)
        {
            if (IsCompleted)
                throw new CheckAlreadyCompletedException(Id, State);

            PerformedAt = performedAt;

            var @event = CheckPerformed.Create(this);
            Apply(@event);
        }

        public void Complete(CheckProcessingResult processingResult, DateTime completedAt)
        {
            ChangeState(CheckState.Complete);
            ProcessingResult = processingResult;
            CompletedAt = completedAt;
            PerformedAt ??= completedAt;

            var @event = CheckCompleted.Create(this);
            Apply(@event);
        }

        public void Error(IEnumerable<CheckError> errors, DateTime completedAt)
        {
            ChangeState(CheckState.Error);
            Errors = errors.ToArray();
            CompletedAt = completedAt;

            var @event = CheckErrorOccurred.Create(this);
            Apply(@event);
        }

        public void Cancel(DateTime cancelledAt)
        {
            if (State != CheckState.Pending)
                InvalidStateException.Create<Check>(Id, CheckState.Pending);

            ChangeState(CheckState.Cancelled);
            CompletedAt = cancelledAt;
            var @event = CheckCancelled.Create(this);
            Apply(@event);
        }

        private void ChangeState(CheckState newState)
        {
            if (IsCompleted)
                throw new CheckAlreadyCompletedException(Id, State);
            if (newState < State)
                throw InvalidStateTransitionException.For<Check>(State, newState);

            State = newState;
        }
    }
}