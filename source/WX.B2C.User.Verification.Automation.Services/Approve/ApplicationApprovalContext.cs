using System;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Monitoring;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Profile;

namespace WX.B2C.User.Verification.Automation.Services.Approve
{
    internal class ApplicationApprovalContext
    {
        public RiskLevel? RiskLevel { get; set; }

        public ApplicationTaskDto[] ApplicationTasks { get; set; }

        public TaskVariantDto[] TaskVariants { get; set; }

        public TriggerDto[] Triggers { get; set; }
    }
}