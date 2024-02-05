using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Automation.Services.Approve;
using WX.B2C.User.Verification.Automation.Services.Extensions;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Core.Contracts.Dtos;

namespace WX.B2C.User.Verification.Automation.Services
{
    public interface IApplicationManager
    {
        Task TryApproveAsync(IReadOnlyCollection<ApplicationDto> applications, string reason);

        Task TryReviewAsync(IReadOnlyCollection<ApplicationDto> applications, string reason);

        Task TryRejectAsync(IReadOnlyCollection<ApplicationDto> applications, string reason);

        Task<IEnumerable<Guid>> TryRejectAsync(IReadOnlyCollection<ApplicationDto> applications, string[] changes, string reason);
    }

    internal class ApplicationManager : IApplicationManager
    {
        private readonly IApplicationApprovalService _applicationApprovalService;
        private readonly IApplicationRejectionService _applicationRejectionService;
        private readonly IBatchCommandPublisher _commandsPublisher;
        private readonly ILogger _logger;

        public ApplicationManager(
            IApplicationApprovalService applicationApprovalService,
            IApplicationRejectionService applicationRejectionService,
            IBatchCommandPublisher commandsPublisher,
            ILogger logger)
        {
            _applicationApprovalService = applicationApprovalService ?? throw new ArgumentNullException(nameof(applicationApprovalService));
            _applicationRejectionService = applicationRejectionService ?? throw new ArgumentNullException(nameof(applicationRejectionService));
            _commandsPublisher = commandsPublisher ?? throw new ArgumentNullException(nameof(commandsPublisher));
            _logger = logger?.ForContext<ApplicationManager>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task TryApproveAsync(IReadOnlyCollection<ApplicationDto> applications, string reason)
        {
            if (applications == null)
                throw new ArgumentNullException(nameof(applications));
            if (string.IsNullOrEmpty(reason))
                throw new ArgumentNullException(nameof(reason));

            var applicationsReadyToApprove = applications.Where(application => application.CanBeApproved()).ToArray();

            var approvalBlockers = await GetApprovalBlockers(applicationsReadyToApprove);
            var (applicationsToApprove, blockedApplications) = applicationsReadyToApprove.SeparateBlocked(approvalBlockers);
            var applicationsReadyForReview = blockedApplications.Where(IsReadyForReview).ToArray();

            var commands = applicationsToApprove
                   .Concat(applicationsReadyForReview)
                   .Select(application => new ApproveApplicationCommand(
                       application.UserId,
                       application.Id,
                       applicationsReadyForReview.Contains(application),
                       reason))
                   .ToArray();

            await _commandsPublisher.PublishAsync(commands);
            LogBlockedApplications(blockedApplications);

            // TODO: Consider this logic
            static bool IsReadyForReview(BlockedApplication application) =>
                application.Blockers.All(blocker => blocker is IncompleteTaskApprovalBlocker { IsManual: true });
        }

        public Task TryReviewAsync(IReadOnlyCollection<ApplicationDto> applications, string reason)
        {
            if (applications == null)
                throw new ArgumentNullException(nameof(applications));
            if (string.IsNullOrEmpty(reason))
                throw new ArgumentNullException(nameof(reason));

            var commands = applications
                   .Where(application => application.CanMoveInReview())
                   .Select(application => new MoveApplicationInReviewCommand(application.UserId, application.Id, reason))
                   .ToArray();

            return _commandsPublisher.PublishAsync(commands);
        }

        public Task TryRejectAsync(IReadOnlyCollection<ApplicationDto> applications, string reason)
        {
            if (applications == null)
                throw new ArgumentNullException(nameof(applications));
            if (string.IsNullOrEmpty(reason))
                throw new ArgumentNullException(nameof(reason));

            var commands = applications
                   .Where(application => application.CanBeRejectedOrCancelled())
                   .Select(application => new RejectApplicationCommand(application.UserId, application.Id, reason))
                   .ToArray();

            return _commandsPublisher.PublishAsync(commands);
        }
        
        public async Task<IEnumerable<Guid>> TryRejectAsync(IReadOnlyCollection<ApplicationDto> applications, string[] changes, string reason)
        {
            var applicationsToReject = await _applicationRejectionService.FindApplicationsToRejectAsync(applications, changes);

            var commands = applicationsToReject
                           .Select(application => new RejectApplicationCommand(application.UserId, application.Id, reason))
                           .ToArray();

            await _commandsPublisher.PublishAsync(commands);

            return applicationsToReject.Select(dto => dto.Id);
        }

        private async Task<Dictionary<Guid, ApprovalBlocker[]>> GetApprovalBlockers(IEnumerable<ApplicationDto> applications)
        {
            var results = new Dictionary<Guid, ApprovalBlocker[]>();

            foreach (var application in applications)
            {
                var blockers = await _applicationApprovalService.GetBlockersAsync(application);
                results.Add(application.Id, blockers);
            }

            return results;
        }

        private void LogBlockedApplications(IEnumerable<BlockedApplication> applications)
        {
            foreach (var application in applications)
            {
                _logger.Information(
                    "Cannot approve application {ApplicationId} for user {UserId} because {@Reasons}.",
                    application.Id,
                    application.UserId,
                    application.Blockers);
            }
        }
    }
}