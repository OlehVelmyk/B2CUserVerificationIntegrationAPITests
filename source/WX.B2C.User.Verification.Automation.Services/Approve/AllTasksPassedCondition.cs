using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Automation.Services.Extensions;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;

namespace WX.B2C.User.Verification.Automation.Services.Approve
{
    internal class AllTasksPassedCondition : IApplicationApprovalCondition
    {
        public bool IsSatisfied(ApplicationApprovalContext context, out List<ApprovalBlocker> approvalBlockers)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var failedTaskBlockers = SelectFailedTaskBlockers(context);
            var incompleteTaskBlockers = SelectIncompleteTaskBlockers(context);
            approvalBlockers = incompleteTaskBlockers.Concat(failedTaskBlockers).ToList();

            return approvalBlockers.Count == 0;
        }

        private static IEnumerable<ApprovalBlocker> SelectFailedTaskBlockers(ApplicationApprovalContext context)
        {
            return context.ApplicationTasks
                          .Where(task => task.IsFailedTask())
                          .Select(task => ApprovalBlocker.FailedTask(task.Type, task.VariantId));
        }

        private static IEnumerable<ApprovalBlocker> SelectIncompleteTaskBlockers(ApplicationApprovalContext context)
        {
            return context.ApplicationTasks
                          .Where(task => task.IsIncompleteTask())
                          .Select(task => ApprovalBlocker.IncompleteTask(task.Type, task.VariantId, IsManualTask(task)));

            bool IsManualTask(ApplicationTaskDto task) =>
                context.TaskVariants
                       .Where(x => x.VariantId == task.VariantId)
                       .Any(x => x.AutoCompletePolicy == AutoCompletePolicy.None);
        }
    }
}