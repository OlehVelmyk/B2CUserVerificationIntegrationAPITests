using System;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal interface ICheckOutputPolicyProvider
    {
        CheckOutputReviewPolicy? Find(CheckType checkDtoType);
    }

    /// <summary>
    /// TODO PHASE 3 - When requirements will be more stable can be moved to check variants or even to separate policy file.
    /// </summary>
    internal class HardcodedCheckOutputPolicyProvider : ICheckOutputPolicyProvider
    {
        public CheckOutputReviewPolicy? Find(CheckType checkType)
        {
            return checkType switch
            {
                CheckType.RiskListsScreening => CheckOutputReviewPolicy.FailReviewRequired,
                CheckType.IdentityDocument => CheckOutputReviewPolicy.FailReviewRequired,
                CheckType.IdentityEnhanced => null,
                CheckType.FacialSimilarity => null,
                CheckType.TaxResidence => null,
                CheckType.IpMatch => null,
                CheckType.FaceDuplication => null,
                CheckType.NameAndDoBDuplication => null,
                CheckType.IdDocNumberDuplication => null,
                CheckType.FraudScreening => null,
                CheckType.Address => null,
                CheckType.SurveyAnswers => null,
                _ => throw new ArgumentOutOfRangeException(nameof(checkType), checkType, "Unsupported check type.")
            };
        }
    }
}