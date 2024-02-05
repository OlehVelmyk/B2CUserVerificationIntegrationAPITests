using System;
using System.Collections.Generic;

namespace WX.B2C.User.Verification.Automation.Services.Approve
{
    internal class RiskLevelEvaluatedCondition : IApplicationApprovalCondition
    {
        public bool IsSatisfied(ApplicationApprovalContext context, out List<ApprovalBlocker> blockers)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            blockers = new List<ApprovalBlocker>();

            if (!context.RiskLevel.HasValue)
                blockers.Add(ApprovalBlocker.RiskLevelNotEvaluated());

            return blockers.Count == 0;
        }
    }
}