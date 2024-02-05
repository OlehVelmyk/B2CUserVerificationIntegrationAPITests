using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers.Tasks
{
    internal interface IDynamicTasksProvider : ITaskVariantProvider
    { }

    internal interface IStaticTasksProvider : ITaskVariantProvider
    { }

    internal interface ITaskVariantProvider
    {
        TaskVariant[] Get(Guid policyId);
        
        TaskVariant Find(Guid policyId, TaskType taskType);
    }
}
