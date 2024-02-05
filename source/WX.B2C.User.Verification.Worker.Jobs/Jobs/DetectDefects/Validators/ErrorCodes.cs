using System.Linq;
using System.Reflection;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.DetectDefects.Validators
{
    internal static class ErrorCodes
    {
        private static readonly string[] _errorCodes;

        static ErrorCodes()
        {
            _errorCodes = GetConstants();
        }
        
        // Application defects
        public const string NoApplication = nameof(NoApplication);
        public const string ApprovedWhenTaskIncompleteOrFailed = nameof(ApprovedWhenTaskIncompleteOrFailed);
        public const string NotApprovedWhenAllTaskCompleted = nameof(NotApprovedWhenAllTaskCompleted);
        public const string ApprovedOrInReviewWhenNoRiskLevel = nameof(ApprovedOrInReviewWhenNoRiskLevel);
        public const string ApplicationPolicyIsNotDefined = nameof(ApplicationPolicyIsNotDefined);

        // Data user defect
        public const string AbsentPassFortProfile = nameof(AbsentPassFortProfile);
        public const string AbsentOnfidoApplicantIdWhenApplicationWasApproved = nameof(AbsentOnfidoApplicantIdWhenApplicationWasApproved);
        public const string AbsentOnfidoApplicantIdWhenIdentityTaskCompleted = nameof(AbsentOnfidoApplicantIdWhenIdentityTaskCompleted);

        public const string AbsentIpAddress = nameof(AbsentIpAddress);
        public const string AbsentTaxResidence = nameof(AbsentTaxResidence);
        public const string AbsentFullName = nameof(AbsentFullName);
        public const string AbsentBirthdate = nameof(AbsentBirthdate);
        public const string AbsentTinWhenUSA = nameof(AbsentTinWhenUSA);
        public const string IdDocumentTypeAbsentWhenNumberExists = nameof(IdDocumentTypeAbsentWhenNumberExists);
        public const string AbsentIdDocNumberWhenIdentityTaskCompleted = nameof(AbsentIdDocNumberWhenIdentityTaskCompleted);
        public const string AbsentIdentityDocumentsWhenIdentityTaskCompleted = nameof(AbsentIdentityDocumentsWhenIdentityTaskCompleted);
        public const string AbsentPofDocumentsWhenPofTaskCompleted = nameof(AbsentPofDocumentsWhenPofTaskCompleted);

        // Task defects
        public const string TaskCompleteWhenCheckNotCompleted = nameof(TaskCompleteWhenCheckNotCompleted);
        public const string TaskCompleteWhenCollectionStepNotComplete = nameof(TaskCompleteWhenCollectionStepNotComplete);
        public const string TaskIncompleteWhenNoBlockers = nameof(TaskIncompleteWhenNoBlockers);
        public const string RudimentTaskCreatedWhenNoApplicationId = nameof(RudimentTaskCreatedWhenNoApplicationId);
        public const string RudimentTaskCreatedWhenWrongApplicationId = nameof(RudimentTaskCreatedWhenWrongApplicationId);
        public const string SeveralTaskCreatedWithOneType = nameof(SeveralTaskCreatedWithOneType);
        // IsPep, IsSanctioned, IsAdverseMedia migrated even if not complete. We need to instruct checks for such users
        public const string RiskListsScreeningIncompleteButNoChecks = nameof(RiskListsScreeningIncompleteButNoChecks);
        public const string IdentityIncompleteButNoChecks = nameof(IdentityIncompleteButNoChecks);
        public const string TaxResidenceCompletedWhenNoTaxResidence = nameof(TaxResidenceCompletedWhenNoTaxResidence);

        // Collection steps defects
        public const string StepInNotRequestedStateWhenDataAbsent = nameof(StepInNotRequestedStateWhenDataAbsent);
        // This is not critical issue, collection step can be re-requested
        public const string StepRequestedWhenDataExists = nameof(StepRequestedWhenDataExists);
        public const string SeveralStepsInOpenStateWithSameXPath = nameof(SeveralStepsInOpenStateWithSameXPath);
        public const string StepCreatedButNotAttached = nameof(StepCreatedButNotAttached);
        public const string StepInReviewWhenReviewNotRequired = nameof(StepInReviewWhenReviewNotRequired);
        public const string AbsentReviewResultWhenCompletedStepRequiresReview = nameof(AbsentReviewResultWhenCompletedStepRequiresReview);
        public const string RudimentReviewResult = nameof(RudimentReviewResult);
        public const string StepsMustBeAttachedOnlyToApplicationTasks = nameof(StepsMustBeAttachedOnlyToApplicationTasks);

        public static string[] AllCodes => _errorCodes;

        private static string[] GetConstants() =>
            typeof(ErrorCodes).GetFields(BindingFlags.Public | BindingFlags.Static |
                                 BindingFlags.FlattenHierarchy)
                      .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                      .Select(info => (string)info.GetValue(null))
                      .OrderBy(s => s)
                      .ToArray();
    }
}