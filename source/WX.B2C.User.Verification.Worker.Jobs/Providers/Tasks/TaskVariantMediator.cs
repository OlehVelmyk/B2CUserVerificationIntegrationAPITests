using System;
using System.Linq;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Worker.Jobs.Providers.Tasks
{
    internal class TaskVariantMediator : ITaskVariantProvider
    {
        private readonly IStaticTasksProvider _staticTasksProvider;
        private readonly IDynamicTasksProvider _dynamicTasksProvider;

        public TaskVariantMediator(IStaticTasksProvider staticTasksProvider, IDynamicTasksProvider dynamicTasksProvider)
        {
            _staticTasksProvider = staticTasksProvider ?? throw new ArgumentNullException(nameof(staticTasksProvider));
            _dynamicTasksProvider = dynamicTasksProvider ?? throw new ArgumentNullException(nameof(dynamicTasksProvider));
        }

        public TaskVariant Find(Guid policyId, TaskType taskType) =>
             _staticTasksProvider.Find(policyId, taskType) ?? _dynamicTasksProvider.Find(policyId, taskType);

        public TaskVariant[] Get(Guid policyId) =>
            _staticTasksProvider.Get(policyId).Concat(_dynamicTasksProvider.Get(policyId)).ToArray();
    }
}
