using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Automation.Services.Validators;
using WX.B2C.User.Verification.Core.Contracts.Automation;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal class ApplicationEventObserver : IApplicationEventObserver
    {
        private readonly IApplicationRejectionValidator _applicationRejectionValidator;
        private readonly IApplicationStorage _applicationStorage;
        private readonly IApplicationManager _applicationManager;

        public ApplicationEventObserver(
            IApplicationRejectionValidator applicationRejectionValidator,
            IApplicationStorage applicationStorage,
            IApplicationManager applicationManager)
        {
            _applicationRejectionValidator = applicationRejectionValidator ?? throw new ArgumentNullException(nameof(applicationRejectionValidator));
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
            _applicationManager = applicationManager ?? throw new ArgumentNullException(nameof(applicationManager));
        }

        public async Task OnTaskIncomplete(Guid taskId)
        {
            var reason = InitiationReasons.RequestReviewApplicationWhenTaskUncompleted(taskId);
            var associatedApplications = await _applicationStorage.FindAssociatedWithTaskAsync(taskId);
            await _applicationManager.TryReviewAsync(ReadyForAutomation(associatedApplications), reason);
            
        }

        public async Task OnTaskPassed(Guid taskId)
        {
            var reason = InitiationReasons.ApproveApplicationWhenLastTaskPassed(taskId);
            var associatedApplications = await _applicationStorage.FindAssociatedWithTaskAsync(taskId);
            await _applicationManager.TryApproveAsync(ReadyForAutomation(associatedApplications), reason);
        }

        public async Task OnTaskFailed(Guid taskId)
        {
            var reason = InitiationReasons.RequestReviewApplicationWhenTaskFailed(taskId);
            var associatedApplications = await _applicationStorage.FindAssociatedWithTaskAsync(taskId);
            await _applicationManager.TryReviewAsync(ReadyForAutomation(associatedApplications), reason);
        }

        public async Task OnCheckFailed(Guid userId, Guid checkId)
        {
            var rejectionReason = await _applicationRejectionValidator.ValidateCheckResultAsync(checkId);
            if (rejectionReason is null)
                return;

            var associatedApplications = await _applicationStorage.FindAsync(userId);
            await _applicationManager.TryRejectAsync(ReadyForAutomation(associatedApplications), rejectionReason);
        }

        public async Task OnDetailsChanged(Guid userId, string[] changes)
        {
            var reason = InitiationReasons.RejectApplicationDueToRejectionPolicy(userId);
            var associatedApplications = await _applicationStorage.FindAsync(userId);
            var readyForAutomation = ReadyForAutomation(associatedApplications);
            var rejected = await _applicationManager.TryRejectAsync(readyForAutomation, changes, reason);

            if (changes.Contains(XPathes.RiskLevel))
            {
                var approveReason = InitiationReasons.ApproveApplicationWhenRiskLevelEvaluated;
                var readyToApprove = readyForAutomation.Where(application => application.State == ApplicationState.Applied && !application.Id.In(rejected)).ToArray();
                await _applicationManager.TryApproveAsync(readyToApprove, approveReason);
            }
        }

        public async Task OnPoiIssuingCountryChanged(Guid userId, string poiIssuingCountry)
        {
            var rejectionReason = await _applicationRejectionValidator.ValidatePoiIssuingCountryAsync(poiIssuingCountry);
            if (rejectionReason is null)
                return;

            var associatedApplications = await _applicationStorage.FindAsync(userId);
            await _applicationManager.TryRejectAsync(ReadyForAutomation(associatedApplications), rejectionReason);
        }

        public async Task OnTriggerCompleted(Guid applicationId, Guid triggerId)
        {
            var reason = InitiationReasons.ApproveApplicationWhenTriggerCompleted(triggerId);
            var application = await _applicationStorage.GetAsync(applicationId);
            await _applicationManager.TryApproveAsync(ReadyForAutomation(application), reason);
        }

        private static IReadOnlyCollection<ApplicationDto> ReadyForAutomation(params ApplicationDto[] applications) =>
            applications.Where(dto => dto.IsAutomating).ToArray();
    }
}
