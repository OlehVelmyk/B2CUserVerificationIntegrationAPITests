using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Domain
{
    public class Initiation
    {
        public string Initiator { get; }
        public string Reason { get; }

        public Initiation(string initiator, string reason)
        {
            Initiator = initiator ?? throw new ArgumentNullException(nameof(initiator));
            Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        }
    }

    public static class Initiators
    {
        public const string System = nameof(System);

        public const string User = nameof(User);

        public const string Admin = nameof(Admin);

        public const string Job = nameof(Job);
    }

    public static class InitiationReasons
    {
        public static string CompleteCollectionStep() =>
            $"Complete collection step.";

        public static string NewCollectionStepAdded(Guid collectionStepId) =>
            $"Incomplete task because new collection step {collectionStepId} was added to task.";

        public static string CompleteTaskWhenCheckCompleted(Guid checkId) => 
            $"Complete task because last check {checkId} completed.";

        public static string IncompleteTaskWhenCheckFailed(Guid checkId) =>
            $"Incomplete task because check {checkId} failed.";

        public static string IncompleteTaskWhenCollectionStepRequired(Guid collectionStepId) =>
            $"Incomplete task because collection step {collectionStepId} became required.";

        public static string CompleteTaskWhenCollectionStepCompleted(Guid collectionStepId) => 
            $"Complete task because last collection step {collectionStepId} completed.";

        public static string ApproveApplicationWhenLastTaskPassed(Guid taskId) =>
            $"Approve application because last task {taskId} is passed.";
        
        public static string ApproveApplicationWhenTriggerCompleted(Guid triggerId) =>
            $"Approve application because trigger {triggerId} completed";

        public const string ApproveApplicationWhenRiskLevelEvaluated =
            "Approve application because risk level evaluated.";

        public static string ApproveApplicationWhenVerificationDetailsChanged() =>
            $"Approve application due to approve policy when verification details updated.";

        public static string RequestReviewApplicationWhenTaskFailed(Guid taskId) =>
            $"Move application in review because task {taskId} is failed.";

        public static string RequestReviewApplicationWhenTaskUncompleted(Guid taskId) =>
            $"Move application in review because task {taskId} became incomplete.";

        public static string NewRequiredTaskAdded(Guid taskVariant) => 
            $"Move application in review because added new required task variant {taskVariant}.";

        public static string ApplicationPolicyIsChangedForUser(Guid userId, Guid newApplicationPolicyId, ProductType productType) =>
            $"Reject application {productType} for user {userId} because new policy is required {newApplicationPolicyId}.";

        public static string RejectApplicationDueToRejectionPolicy(Guid userId) =>
            $"Reject application for user {userId} because of rejection policy.";

        public static string Trigger(Guid triggerVariantId) =>
            $"Trigger {triggerVariantId} fired.";

        public static string AddCollectionStepDueToFailResult(Guid checkId) =>
            $"Add collection step due to fail policy in check: {checkId}";

        public static string ResubmitCollectionStepDueToFailResult(Guid checkId) =>
            $"Resubmit collection step and check due to fail policy in check: {checkId}";

        public static string RequestCollectionStepByPolicy =>
            "Request collection step according to task policy.";

        public static string RequestCheckByPolicy =>
            "Request check according to task policy.";

        public static string AddRequiredTasksByPolicy => 
            "Added initial required tasks from policy.";

        public static string ApplicationIsBuilt =>
            "Application is finally built according to policy.";

        public static string PolicyChanged(Guid oldPolicyId, Guid newPolicyId) =>
            $"Verification policy changed from {oldPolicyId} to {newPolicyId}.";

        public static string CreateTaskByPolicy => 
            "Create task from policy.";

        public static string ArchiveDocument =>
            "Archive document.";
    }
}