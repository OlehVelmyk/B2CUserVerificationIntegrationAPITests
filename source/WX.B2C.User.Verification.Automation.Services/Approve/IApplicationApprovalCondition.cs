using System.Collections.Generic;

namespace WX.B2C.User.Verification.Automation.Services.Approve
{
    internal interface IApplicationApprovalCondition
    {
        bool IsSatisfied(ApplicationApprovalContext context, out List<ApprovalBlocker> blockers);
    }
}