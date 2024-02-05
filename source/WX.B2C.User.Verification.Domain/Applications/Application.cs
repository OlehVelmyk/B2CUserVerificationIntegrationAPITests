using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Domain.Events;
using WX.B2C.User.Verification.Domain.Exceptions;
using WX.B2C.User.Verification.Domain.Extensions;
using WX.B2C.User.Verification.Domain.Shared;

namespace WX.B2C.User.Verification.Domain.Models
{
    public class Application : AggregateRoot
    {
        private readonly HashSet<ApplicationTask> _requiredTasks;
        private readonly HashSet<string> _decisionReasons;

        public Application(Guid id,
                           Guid userId,
                           Guid policyId,
                           ProductType productType,
                           ApplicationState state,
                           ApplicationState? previousState,
                           ApplicationTask[] requiredTasks,
                           string[] decisionReasons,
                           bool isAutomating)
            : base(id)
        {
            UserId = userId;
            PolicyId = policyId;
            ProductType = productType;
            State = state;
            PreviousState = previousState;
            IsAutomating = isAutomating;
            _decisionReasons = decisionReasons?.ToHashSet() ?? new HashSet<string>();
            _requiredTasks = requiredTasks?.ToHashSet() ?? new HashSet<ApplicationTask>();
        }

        public Guid UserId { get; }

        public Guid PolicyId { get; }

        public ProductType ProductType { get; }

        public ApplicationState State { get; private set; }

        public ApplicationState? PreviousState { get; private set; }

        public IReadOnlyCollection<string> DecisionReasons => _decisionReasons;

        public IReadOnlyCollection<ApplicationTask> RequiredTasks => _requiredTasks;

        public bool IsAutomating { get; private set; }

        private bool CanRevertState =>
            (State is ApplicationState.Rejected && PreviousState is ApplicationState.Applied) ||
            (State is ApplicationState.Cancelled && PreviousState is ApplicationState.InReview or ApplicationState.Approved);

        private bool CanApprove => RequiredTasks.All(task => task is { State: TaskState.Completed });

        private IEnumerable<ApplicationTask> IncompleteTasks =>
            RequiredTasks.Where(task => task is not { State: TaskState.Completed });

        public static Application Create(Guid userId, Guid policyId, ProductType productType, Initiation initiation)
        {
            var applicationId = Guid.NewGuid();
            var requiredTasks = Array.Empty<ApplicationTask>();

            var application = new Application(applicationId,
                                              userId,
                                              policyId,
                                              productType,
                                              ApplicationState.Applied,
                                              null,
                                              requiredTasks,
                                              null,
                                              false);

            var @event = ApplicationRegistered.Create(application, initiation);
            application.Apply(@event);
            return application;
        }

        public void AddRequiredTask(VerificationTask task, Initiation initiation)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (initiation == null)
                throw new ArgumentNullException(nameof(initiation));

            if (task.IsExpired)
                throw new TaskExpiredException(Id, task.Id);

            if (task.UserId != UserId)
                throw new UserMismatchedException(this, task);

            if (_requiredTasks.Any(t => t.Id == task.Id))
                return;

            if (_requiredTasks.Any(t => t.TaskType == task.Type))
                throw new ApplicationTaskAlreadyExistsException(task, _requiredTasks.First(t => t.TaskType == task.Type).Id);

            var applicationTask = ApplicationTask.Create(task);
            if (_requiredTasks.Add(applicationTask))
                Apply(ApplicationRequiredTaskAdded.Create(this, applicationTask, initiation));

            var isTaskPassed = task.State == TaskState.Completed && task.Result == TaskResult.Passed;
            if (State == ApplicationState.Approved && !isTaskPassed)
                RequestReview(new Initiation(Initiators.System, InitiationReasons.NewRequiredTaskAdded(task.Id)));
        }

        public void Automate(Initiation initiation)
        {
            if (IsAutomating)
                return;

            IsAutomating = true;

            Apply(ApplicationAutomated.Create(this, initiation));
        }

        public void Approve(Initiation initiation)
        {
            if (initiation == null)
                throw new ArgumentNullException(nameof(initiation));

            if (!CanApprove)
                throw new ApproveApplicationException(Id, IncompleteTasks.Select(task => task.Id));

            ChangeState(ApplicationState.Approved, initiation);
        }

        public void Reject(Initiation initiation)
        {
            if (initiation == null)
                throw new ArgumentNullException(nameof(initiation));

            var newState = State is ApplicationState.Applied or ApplicationState.Rejected
                ? ApplicationState.Rejected
                : ApplicationState.Cancelled;

            AddDecisionReasons(initiation.Reason);
            ChangeState(newState, initiation);
        }

        public void RequestReview(Initiation initiation)
        {
            if (initiation == null)
                throw new ArgumentNullException(nameof(initiation));

            ChangeState(ApplicationState.InReview, initiation);
        }

        public void RevertDecision(Initiation initiation)
        {
            if (initiation == null)
                throw new ArgumentNullException(nameof(initiation));

            if (!PreviousState.HasValue)
                throw new InconsistentRevertDecisionOperationException(Id);

            if (!CanRevertState)
                throw InvalidStateTransitionException.For<Application>(State, PreviousState.Value);

            _decisionReasons.Clear();

            (State, PreviousState) = (PreviousState.Value, State);
            Apply(ApplicationStateChanged.Create(this, PreviousState.Value, initiation));
        }

        private void ChangeState(ApplicationState newState, Initiation initiation)
        {
            if (newState == State)
                return;

            if (!State.CanTransitTo(newState))
                throw InvalidStateTransitionException.For<Application>(State, newState);

            PreviousState = State;
            State = newState;

            Apply(ApplicationStateChanged.Create(this, PreviousState.Value, initiation));
        }

        private void AddDecisionReasons(params string[] decisionReasons)
        {
            foreach (var decisionReason in decisionReasons)
            {
                _decisionReasons.Add(decisionReason);
            }
        }
    }
}