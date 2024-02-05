using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Optional;
using Serilog;
using WX.B2C.User.Verification.Automation.Services.Validators;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Profile;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal class ApplicationMigrationService : IApplicationMigrationService
    {
        private readonly ITaskStorage _taskStorage;
        private readonly IApplicationService _applicationService;
        private readonly ITaskService _taskService;
        private readonly ICollectionStepService _collectionStepService;
        private readonly ICheckService _checkService;
        private readonly IProfileService _profileService;
        private readonly ILogger _logger;

        public ApplicationMigrationService(ITaskStorage taskStorage,
                                           IApplicationService applicationService, 
                                           ITaskService taskService,
                                           ICollectionStepService collectionStepService,
                                           ICheckService checkService,
                                           IProfileService profileService,
                                           ILogger logger)
        {
            _taskStorage = taskStorage ?? throw new ArgumentNullException(nameof(taskStorage));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _collectionStepService = collectionStepService ?? throw new ArgumentNullException(nameof(collectionStepService));
            _checkService = checkService ?? throw new ArgumentNullException(nameof(checkService));
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            _logger = logger?.ForContext<ApplicationMigrationService>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ChangePolicyAsync(ApplicationDto application, Guid? newPolicyId, bool isAvailable)
        {
            var rejectionReason = InitiationDto.CreateSystem(ApplicationRejectionReasons.VerificationPolicyChanged);
            await _applicationService.RejectAsync(application.Id, rejectionReason);

            if (newPolicyId is null || !isAvailable)
            {
                _logger.ForContext(nameof(ApplicationDto.UserId), application.UserId)
                       .ForContext(nameof(newPolicyId), newPolicyId)
                       .ForContext(nameof(isAvailable), isAvailable)
                       .Information("Verification context has changed, but there are no available policies for new context, userId: {userId}.");
                return;
            }

            var applicationTasks = await _taskStorage.GetByApplicationIdAsync(application.Id);

            await CancelRequestedStepsAsync(applicationTasks, rejectionReason);
            await RejectReviewingStepsAsync(applicationTasks, rejectionReason);
            await CancelPendingChecksAsync(applicationTasks);
            await FailIncompleteTasks(applicationTasks, rejectionReason);
            await ResetVerificationDataAsync(application.UserId, rejectionReason);

            await _applicationService.RegisterAsync(application.UserId, new NewVerificationApplicationDto
            {
                PolicyId = newPolicyId.Value,
                ProductType = application.ProductType
            }, InitiationDto.CreateSystem(InitiationReasons.PolicyChanged(application.PolicyId, newPolicyId.Value)));
        }

        private Task CancelRequestedStepsAsync(IEnumerable<TaskDto> applicationTasks, InitiationDto reason)
        {
            var requestedSteps = applicationTasks
                                 .SelectMany(task => task.CollectionSteps, (_, step) => step)
                                 .Where(step => step.State is CollectionStepState.Requested)
                                 .Select(step => step.Id)
                                 .Distinct()
                                 .ToArray();

            return requestedSteps.Foreach(stepId => _collectionStepService.CancelAsync(stepId, reason));
        }

        private Task RejectReviewingStepsAsync(IEnumerable<TaskDto> applicationTasks, InitiationDto reason)
        {
            var requestedSteps = applicationTasks
                                 .SelectMany(task => task.CollectionSteps, (_, step) => step)
                                 .Where(step => step.State is CollectionStepState.InReview)
                                 .Select(step => step.Id)
                                 .Distinct()
                                 .ToArray();

            return requestedSteps.Foreach(stepId => _collectionStepService.ReviewAsync(stepId, CollectionStepReviewResult.Rejected, reason));
        }

        private Task CancelPendingChecksAsync(IEnumerable<TaskDto> applicationTasks)
        {
            var pendingChecks = applicationTasks
                                .SelectMany(task => task.Checks, (_, check) => check)
                                .Where(check => check.State is CheckState.Pending)
                                .Select(check => check.Id)
                                .Distinct()
                                .ToArray();

            return pendingChecks.Foreach(checkId => _checkService.CancelAsync(checkId));
        }

        private Task FailIncompleteTasks(IEnumerable<TaskDto> applicationTasks, InitiationDto reason)
        {
            var incompleteTasks = applicationTasks.Where(task => task.State == TaskState.Incomplete);
            return incompleteTasks.Foreach(task => _taskService.CompleteAsync(task.Id, TaskResult.Failed, reason));
        }

        private Task ResetVerificationDataAsync(Guid userId, InitiationDto reason) =>
            _profileService.UpdateAsync(userId, new VerificationDetailsPatch
            {
                Tin = Option.Some<TinDto>(default),
                TaxResidence = Option.Some<string[]>(default),
                Nationality = Option.Some<string>(default),
                IdDocumentNumber = Option.Some<IdDocumentNumberDto>(default),
                PlaceOfBirth = Option.Some<string>(default),
                PoiIssuingCountry = Option.Some<string>(default),
                ResolvedCountryCode = Option.Some<string>(default),
                RiskLevel = Option.Some<RiskLevel?>(default),
                IsAdverseMedia = Option.Some<bool?>(default),
                IsIpMatched = Option.Some<bool?>(default),
                IsSanctioned = Option.Some<bool?>(default),
                IsPep = Option.Some<bool?>(default),
                ComprehensiveIndex = Option.Some<int?>(default)
            }, reason);
    }
}
