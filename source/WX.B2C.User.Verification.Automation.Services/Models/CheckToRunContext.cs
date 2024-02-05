using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal class CheckToRunContext
    {
        public PendingCheck Check { get; private set; }

        public IDictionary<string, CollectionStepDto> RequestedSteps { get; private set; }

        public IReadOnlyDictionary<string, object> InputData { get; private set; }

        public ExternalProfileDto ExternalProfile { get; private set; }

        public int Priority { get; private set; }

        public TaskPriorityGroupDto[] PriorityGroups { get; private set; }

        public static CheckToRunContext Create(PendingCheck check)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            return new CheckToRunContext { Check = check };
        }

        public CheckToRunContext With(IDictionary<string, CollectionStepDto> requestedSteps)
        {
            RequestedSteps = requestedSteps ?? throw new ArgumentNullException(nameof(requestedSteps));
            return this;
        }

        public CheckToRunContext With(ExternalProfileDto externalProfile)
        {
            ExternalProfile = externalProfile ?? throw new ArgumentNullException(nameof(externalProfile));
            return this;
        }

        public CheckToRunContext With(Dictionary<string, object> inputData)
        {
            InputData = inputData ?? throw new ArgumentNullException(nameof(inputData));
            return this;
        }

        public CheckToRunContext With(TaskPriorityGroupDto[] priorityGroups)
        {
            PriorityGroups = priorityGroups ?? throw new ArgumentNullException(nameof(priorityGroups));
            return this;
        }

        public CheckToRunContext With(int priority)
        {
            Priority = priority;
            return this;
        }
    }
}