using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Automation.Services.Extensions
{
    internal static class TaskExtensions
    {
        public static bool CanBeIncomplete(this TaskDto task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            return task.State == TaskState.Completed;
        }

        public static bool CanBeCompleted(this TaskDto task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            return task.State == TaskState.Incomplete;
        }
    }
}
