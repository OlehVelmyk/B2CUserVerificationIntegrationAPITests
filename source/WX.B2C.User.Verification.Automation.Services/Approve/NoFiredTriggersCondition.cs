using System;
using System.Collections.Generic;
using System.Linq;
using WX.B2C.User.Verification.Automation.Services.Extensions;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.Triggers;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services.Approve
{
    internal class NoFiredTriggersCondition : IApplicationApprovalCondition
    {
        public bool IsSatisfied(ApplicationApprovalContext context, out List<ApprovalBlocker> blockers)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            blockers = new List<ApprovalBlocker>();

            if (context.Triggers.Any(dto => dto.State == TriggerState.Fired))
                blockers.Add(ApprovalBlocker.TriggerIsFiring());

            return blockers.Count == 0;
        }
    }
}