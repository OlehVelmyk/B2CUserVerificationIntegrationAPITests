using System;
using System.Collections.Generic;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Integration.Tests.Models
{
    internal class PolicyTasksSpecimen<TPolicyTask> : List<TPolicyTask>, IReadOnlyCollection<TPolicyTask>
        where TPolicyTask : PolicyTaskSpecimen
    {
        public PolicyTasksSpecimen(IEnumerable<TPolicyTask> items)
            : base(items)
        { }
    }

    internal class IdentityPolicyTaskSpecimen : PolicyTaskSpecimen
    {
        public IdentityPolicyTaskSpecimen(Guid policyId, Guid taskVariantId, TaskType taskType)
        : base(policyId, taskVariantId, taskType)
        { }
    }
    
    internal class GbPoFPolicyTaskSpecimen : PolicyTaskSpecimen
    {
        public GbPoFPolicyTaskSpecimen(Guid policyId, Guid taskVariantId, TaskType taskType)
        : base(policyId, taskVariantId, taskType)
        { }
    }

    internal class UsPolicyTaskSpecimen : PolicyTaskSpecimen
    {
        public UsPolicyTaskSpecimen(Guid policyId, Guid taskVariantId, TaskType taskType)
        : base(policyId, taskVariantId, taskType)
        { }
    }

    internal class GbPolicyTaskSpecimen : PolicyTaskSpecimen
    {
        public GbPolicyTaskSpecimen(Guid policyId, Guid taskVariantId, TaskType taskType)
            :base(policyId, taskVariantId, taskType)
        { }
    }

    internal class PolicyTaskSpecimen
    {
        public PolicyTaskSpecimen(Guid policyId, Guid taskVariantId, TaskType taskType)
        {
            PolicyId = policyId;
            TaskVariantId = taskVariantId;
            TaskType = taskType;
        }

        public Guid PolicyId { get; private set; }

        public Guid TaskVariantId { get; private set; }

        public TaskType TaskType { get; private set; }
    }
}
