using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Automation.Services.Extensions;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Automation.Services
{
    public interface ITaskCompletionService
    {
        Task<bool> CanBePassedAsync(TaskDto task);
    }

    internal class TaskCompletionService : ITaskCompletionService
    {
        private readonly IVerificationPolicyStorage _verificationPolicyStorage;

        public TaskCompletionService(IVerificationPolicyStorage verificationPolicyStorage)
        {
            _verificationPolicyStorage = verificationPolicyStorage ?? throw new ArgumentNullException(nameof(verificationPolicyStorage));
        }

        public async Task<bool> CanBePassedAsync(TaskDto task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (!task.CanBeCompleted())
                return false;

            if (!AreCollectionStepsSatisfied(task.CollectionSteps))
                return false;

            if (!HasPassedInstructedChecks(task.Checks))
                return false;

            var autoCompletionPolicy = await _verificationPolicyStorage.GetTaskAutoCompletePolicyAsync(task.VariantId);
            if (!CanAutoComplete(autoCompletionPolicy))
                return false;

            return true;
        }

        private static bool CanAutoComplete(AutoCompletePolicy autoCompletionPolicy)
        {
            return autoCompletionPolicy switch
            {
                AutoCompletePolicy.None => false,
                AutoCompletePolicy.EveryTime => true,
                AutoCompletePolicy.OneTime => throw new NotImplementedException("One time completion is not implemented yet"),
                _ => throw new ArgumentOutOfRangeException(nameof(autoCompletionPolicy), autoCompletionPolicy, "Unsupported auto-completion policy.")
            };
        }

        private static bool AreCollectionStepsSatisfied(IEnumerable<TaskCollectionStepDto> collectionSteps)
        {
            if (collectionSteps == null)
                throw new ArgumentNullException(nameof(collectionSteps));

            return collectionSteps.All(IsSatisfied);

            static bool IsSatisfied(TaskCollectionStepDto step) =>
                IsRequiredAndCompleted(step) && (IsNotReviewNeeded(step) || IsReviewNeededAndApproved(step)) || IsNotRequired(step);

            static bool IsRequiredAndCompleted(TaskCollectionStepDto step) =>
                step is { IsRequired: true, State: CollectionStepState.Completed };

            static bool IsReviewNeededAndApproved(TaskCollectionStepDto step) =>
                step is { IsReviewNeeded: true, ReviewResult: CollectionStepReviewResult.Approved };

            static bool IsNotReviewNeeded(TaskCollectionStepDto step) =>
                step is { IsReviewNeeded: false };

            static bool IsNotRequired(TaskCollectionStepDto step) =>
                step is { IsRequired: false };
        }

        private static bool HasPassedInstructedChecks(IEnumerable<TaskCheckDto> taskChecks)
        {
            if (taskChecks == null)
                throw new ArgumentNullException(nameof(taskChecks));

            return taskChecks
                   .GroupBy(
                       taskCheck => taskCheck.VariantId,
                       (variantId, checks) => new
                       {
                           VariantId = variantId,
                           IsLatestPassed = IsLatestCheckPassed(checks)
                       })
                   .All(x => x.IsLatestPassed);

            static bool IsLatestCheckPassed(IEnumerable<TaskCheckDto> performedChecks) =>
                performedChecks.OrderByDescending(check => check.CreatedAt).Take(1).Any(IsPassedCheck);

            static bool IsPassedCheck(TaskCheckDto check) =>
                check is { State: CheckState.Complete, Result: CheckResult.Passed };
        }
    }
}