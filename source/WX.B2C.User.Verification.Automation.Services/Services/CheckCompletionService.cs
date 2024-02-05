using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Automation.Services.Conditions;
using WX.B2C.User.Verification.Automation.Services.Extensions;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Conditions;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Ticket;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Checks;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services
{
    public interface ICheckCompletionService
    {
        Task TryExtractDataAsync(CheckDto check);

        Task TryApplyFailPolicyAsync(CheckDto check);
    }

    /// <summary>
    /// TODO PHASE 2 too many dependencies - refactor
    /// </summary>

    internal class CheckCompletionService : ICheckCompletionService
    {
        private readonly IVerificationPolicyStorage _policyStorage;
        private readonly IProfileService _profileService;
        private readonly ITaskStorage _taskStorage;
        private readonly ITaskService _taskService;
        private readonly ITicketService _ticketService;
        private readonly ICollectionStepStorage _collectionStepStorage;
        private readonly ICollectionStepService _collectionStepService;
        private readonly ICheckStorage _checkStorage;
        private readonly ICheckSelectionService _checkSelectionService;
        private readonly ICheckManager _checkManager;
        private readonly IConditionsFactory _conditionsFactory;
        private readonly ICheckOutputExtractionService _checkOutputExtractionService;
        private readonly ICheckOutputPolicyProvider _checkOutputPolicyProvider;
        private readonly ILogger _logger;

        public CheckCompletionService(IVerificationPolicyStorage policyStorage,
                                      IProfileService profileService,
                                      ITaskStorage taskStorage,
                                      ITaskService taskService,
                                      ITicketService ticketService,
                                      ICollectionStepStorage collectionStepStorage,
                                      ICollectionStepService collectionStepService,
                                      ICheckStorage checkStorage,
                                      ICheckSelectionService checkSelectionService,
                                      ICheckManager checkManager,
                                      IConditionsFactory conditionsFactory,
                                      ICheckOutputExtractionService checkOutputExtractionService,
                                      ICheckOutputPolicyProvider checkOutputPolicyProvider,
                                      ILogger logger)
        {
            _policyStorage = policyStorage ?? throw new ArgumentNullException(nameof(policyStorage));
            _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            _taskStorage = taskStorage ?? throw new ArgumentNullException(nameof(taskStorage));
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _collectionStepStorage = collectionStepStorage ?? throw new ArgumentNullException(nameof(collectionStepStorage));
            _collectionStepService = collectionStepService ?? throw new ArgumentNullException(nameof(collectionStepService));
            _checkStorage = checkStorage ?? throw new ArgumentNullException(nameof(checkStorage));
            _checkSelectionService = checkSelectionService ?? throw new ArgumentNullException(nameof(checkSelectionService));
            _checkManager = checkManager ?? throw new ArgumentNullException(nameof(checkManager));
            _conditionsFactory = conditionsFactory ?? throw new ArgumentNullException(nameof(conditionsFactory));
            _checkOutputExtractionService = checkOutputExtractionService ?? throw new ArgumentNullException(nameof(checkOutputExtractionService));
            _checkOutputPolicyProvider = checkOutputPolicyProvider ?? throw new ArgumentNullException(nameof(checkOutputPolicyProvider));
            _logger = logger?.ForContext<CheckCompletionService>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task TryExtractDataAsync(CheckDto check)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            if (check.OutputData == null)
                return Task.CompletedTask;

            var policy = _checkOutputPolicyProvider.Find(check.Type);
            var isReviewRequired = policy switch
            {
                CheckOutputReviewPolicy.ReviewRequired => true,
                CheckOutputReviewPolicy.FailReviewRequired when check.Result == CheckResult.Failed => true,
                _ => false
            };

            if (isReviewRequired)
                return Task.CompletedTask;

            if (!_checkOutputExtractionService.TryExtract(check, out var checkOutputData))
                return Task.CompletedTask;

            var verificationPatch = new VerificationDetailsPatch
            {
                IdDocumentNumber = checkOutputData.Find<IdDocumentNumberDto>(nameof(VerificationProperty.IdDocumentNumber)),
                Nationality = checkOutputData.Find<string>(nameof(VerificationProperty.Nationality)),
                IsPep = checkOutputData.Find<bool?>(nameof(VerificationProperty.IsPep)),
                IsAdverseMedia = checkOutputData.Find<bool?>(nameof(VerificationProperty.IsAdverseMedia)),
                IsSanctioned = checkOutputData.Find<bool?>(nameof(VerificationProperty.IsSanctioned)),
                PoiIssuingCountry = checkOutputData.Find<string>(nameof(VerificationProperty.PoiIssuingCountry)),
                PlaceOfBirth = checkOutputData.Find<string>(nameof(VerificationProperty.PlaceOfBirth)),
                ComprehensiveIndex = checkOutputData.Find<int?>(nameof(VerificationProperty.ComprehensiveIndex)),
                IsIpMatched = checkOutputData.Find<bool?>(nameof(VerificationProperty.IsIpMatched)),
                ResolvedCountryCode = checkOutputData.Find<string>(nameof(VerificationProperty.ResolvedCountryCode))
            };

            var initiationDto = InitiationDto.CreateSystem($"Update verification details according to check output {check.Id}");
            return _profileService.UpdateAsync(check.UserId, verificationPatch, initiationDto);
        }

        public async Task TryApplyFailPolicyAsync(CheckDto check)
        {
            if (check == null)
                throw new ArgumentNullException(nameof(check));

            // Like additional guard we need to check if we implement all required handlers. So we will throw exception if handler not found, when policy expects it.
            // Later maybe we will change this logic for optimization purpose.
            var failPolicy = await _policyStorage.FindCheckFailPolicyAsync(check.Variant.Id);
            if (failPolicy == null)
            {
                _logger.ForContext(nameof(check.Id), check.Id)
                       .Information(
                           "No fail policy found for check with variantId: {variantId}, so processing terminated.",
                           check.Variant.Id);
                return;
            }

            if (failPolicy.Condition != null)
            {
                var context = PrepareConditionContext(check, failPolicy);
                var condition = _conditionsFactory.Create(failPolicy.Condition);
                if (!condition.IsSatisfied(context))
                {
                    _logger.ForContext(nameof(check), check)
                           .Information(
                               "Check fail policy condition is not satisfied for check with id: {checkId}, so processing terminated.",
                               check.Id);
                    return;
                }
            }

            await ApplyFailPolicyAsync(check, failPolicy);
        }

        private Dictionary<string, object> PrepareConditionContext(CheckDto check, CheckFailPolicy failPolicy)
        {
            if (failPolicy.Condition.Type == ConditionType.MatchDecision)
            {
                return new Dictionary<string, object>
                {
                    [MatchDecisionCondition.DecisionRequiredData] = check.Decision
                };
            }

            if (!_checkOutputExtractionService.TryExtract(check, out var checkOutputData))
                throw new NotImplementedException($"Cannot find fail handler for {check.Type}:{check.Variant.Provider}");

            return new Dictionary<string, object>(checkOutputData);
        }

        private Task ApplyFailPolicyAsync(CheckDto check, CheckFailPolicy failPolicy) => failPolicy switch
        {
            AddCollectionStepFailResult failResult => AddCollectionStepAsync(check, failResult.Step),
            ResubmitCollectionStepFailResult failResult => ResubmitCollectionStepAsync(check, failResult.Step),
            InstructCheckFailResult _ => throw new NotImplementedException("Adding check according to check policy is not possible for now."),
            _ => throw new ArgumentOutOfRangeException(nameof(failPolicy), failPolicy, "Unsupported check fail policy."),
        };

        private async Task AddCollectionStepAsync(CheckDto check, PolicyCollectionStep stepVariant)
        {
            var reason = InitiationReasons.AddCollectionStepDueToFailResult(check.Id);
            var initiation = InitiationDto.CreateSystem(reason);

            var collectionStep = await _collectionStepStorage.FindAsync(check.UserId, stepVariant.XPath);
            var stepId = collectionStep?.Id ?? await RequestCollectionStepAsync(check, stepVariant, initiation);
            await AddCollectionStepToTasks(stepId, check.Id, initiation);
        }

        private async Task ResubmitCollectionStepAsync(CheckDto check, PolicyCollectionStep stepVariant)
        {
            if (await IsExceededResubmitAttempts(check.UserId, check.Variant.Id))
            {
                _logger
                    .ForContext(nameof(check), check)
                    .Information("Max resubmit attempts reached for check variant: {VariantId}, so processing terminated.",
                        check.Variant.Id);

                var ticketContext = new ExceededCheckResubmitAttemptsTicketContext
                {
                    UserId = check.UserId,
                    CheckId = check.Id,
                    Type = check.Type,
                    Provider = check.Variant.Provider
                };
                await _ticketService.SendAsync(ticketContext);
                return;
            }

            var reason = InitiationReasons.ResubmitCollectionStepDueToFailResult(check.Id);
            var initiation = InitiationDto.CreateSystem(reason);

            var stepId = await RequestCollectionStepAsync(check, stepVariant, initiation);
            await AddCollectionStepToTasks(stepId, check.Id, initiation);
            await TryInstructAsync(check.UserId, stepVariant.XPath, reason);
        }

        private async Task<bool> IsExceededResubmitAttempts(Guid userId, Guid variantId)
        {
            var checkVariant = await _policyStorage.FindCheckInfoAsync(variantId);
            var previousChecks = await _checkStorage.GetAsync(userId, new[] { variantId });
            return previousChecks.Where(IsResubmittedCheck).Count() >= checkVariant.MaxAttempts;

            static bool IsResubmittedCheck(CheckDto check) =>
                check is { Result: CheckResult.Failed, Decision: CheckDecisions.Resubmit };
        }

        private async Task AddCollectionStepToTasks(Guid stepId, Guid checkId, InitiationDto initiation)
        {
            var relatedTasks = await _taskStorage.FindByCheckIdAsync(checkId);

            await relatedTasks.Where(IsNotRelatedToStep)
                              .Select(AddCollectionStepToTask)
                              .WhenAll();

            bool IsNotRelatedToStep(TaskDto task) => task.CollectionSteps.All(taskStep => taskStep.Id != stepId);
            Task AddCollectionStepToTask(TaskDto task) => _taskService.AddCollectionStepsAsync(task.Id, new[] { stepId }, initiation);
        }

        private Task<Guid> RequestCollectionStepAsync(CheckDto checkDto, PolicyCollectionStep stepVariant, InitiationDto initiation)
        {
            return _collectionStepService.RequestAsync(checkDto.UserId,
                new NewCollectionStepDto
                {
                    IsRequired = stepVariant.IsRequired,
                    IsReviewNeeded = stepVariant.IsReviewNeeded,
                    XPath = stepVariant.XPath
                }, initiation);
        }

        private async Task TryInstructAsync(Guid userId, string xPath, string reason)
        {
            var acceptanceChecks = await _checkSelectionService.GetAcceptanceChecksAsync(userId);
            var affectedChecks = acceptanceChecks.Where(check => check.Parameters.MatchesXPath(xPath));
            await _checkManager.TryInstructAsync(userId, affectedChecks, reason);
        }
    }
}
