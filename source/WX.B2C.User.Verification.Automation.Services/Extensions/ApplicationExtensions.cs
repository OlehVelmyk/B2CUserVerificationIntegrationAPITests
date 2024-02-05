using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Automation.Services.Approve;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Automation.Services.Extensions
{
    internal static class ApplicationExtensions
    {
        public static bool CanBeApproved(this ApplicationDto application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            return application.State is ApplicationState.Applied or ApplicationState.InReview;
        }

        public static bool CanBeRejected(this ApplicationDto application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            return application.State is ApplicationState.Applied;
        }

        public static bool CanBeRejectedOrCancelled(this ApplicationDto application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            return application.State is not ApplicationState.Rejected and not ApplicationState.Cancelled;
        }

        public static bool CanMoveInReview(this ApplicationDto application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            return application.State is ApplicationState.Approved;
        }

        public static bool IsIncompleteTask(this ApplicationTaskDto task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            return task.State == TaskState.Incomplete;
        }

        public static bool IsFailedTask(this ApplicationTaskDto task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            return task.State == TaskState.Completed && task.Result == TaskResult.Failed;
        }

        public static (ApplicationDto[], BlockedApplication[]) SeparateBlocked(
            this ApplicationDto[] applications,
            IDictionary<Guid, ApprovalBlocker[]> approvalBlockers)
        {
            if (applications == null)
                throw new ArgumentNullException(nameof(applications));
            if (approvalBlockers == null)
                throw new ArgumentNullException(nameof(approvalBlockers));

            var notBlockedApplications = applications
                .Join(
                    approvalBlockers.Where(x => !x.Value.Any()),
                    application => application.Id,
                    pair => pair.Key,
                    (application, _) => application);

            var blockedApplications = applications
                .Join(
                    approvalBlockers.Where(x => x.Value.Any()),
                    application => application.Id,
                    pair => pair.Key,
                    (application, pair) => new BlockedApplication(application, pair.Value));

            return (notBlockedApplications.ToArray(), blockedApplications.ToArray());
        }
    }
}