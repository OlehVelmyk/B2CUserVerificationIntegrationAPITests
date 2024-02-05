using System;
using WX.B2C.User.Verification.Automation.Services.Approve;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal class BlockedApplication : ApplicationDto
    {
        public BlockedApplication(ApplicationDto application, ApprovalBlocker[] blockers)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            Id = application.Id;
            UserId = application.UserId;
            PolicyId = application.PolicyId;
            State = application.State;
            PreviousState = application.PreviousState;
            Blockers = blockers ?? throw new ArgumentNullException(nameof(blockers));
        }

        public ApprovalBlocker[] Blockers { get; set; }
    }
}