using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Commands;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.XPath;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal class ApplicationBuilder : IApplicationBuilder
    {
        private readonly IVerificationPolicyStorage _policyStorage;
        private readonly IExternalProfileStorage _externalProfileStorage;
        private readonly ICheckProviderService _checkProviderService;
        private readonly IProfileProviderFactory _profileProviderFactory;
        private readonly IExternalProviderTypeMapper _externalProviderTypeMapper;
        private readonly IXPathParser _xPathParser;
        private readonly IIdempotentGuidGenerator _idGenerator;
        private readonly IBatchCommandPublisher _commandsPublisher;

        public ApplicationBuilder(IVerificationPolicyStorage policyStorage,
                                  IExternalProfileStorage externalProfileStorage,
                                  ICheckProviderService checkProviderService,
                                  IProfileProviderFactory profileProviderFactory,
                                  IExternalProviderTypeMapper externalProviderTypeMapper,
                                  IXPathParser xPathParser,
                                  IIdempotentGuidGenerator idGenerator,
                                  IBatchCommandPublisher commandsPublisher)
        {
            _policyStorage = policyStorage ?? throw new ArgumentNullException(nameof(policyStorage));
            _externalProfileStorage = externalProfileStorage ?? throw new ArgumentNullException(nameof(externalProfileStorage));
            _checkProviderService = checkProviderService ?? throw new ArgumentNullException(nameof(checkProviderService));
            _profileProviderFactory = profileProviderFactory ?? throw new ArgumentNullException(nameof(profileProviderFactory));
            _xPathParser = xPathParser ?? throw new ArgumentNullException(nameof(xPathParser));
            _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
            _commandsPublisher = commandsPublisher ?? throw new ArgumentNullException(nameof(commandsPublisher));
            _externalProviderTypeMapper = externalProviderTypeMapper ?? throw new ArgumentNullException(nameof(externalProviderTypeMapper));
        }

        public async Task BuildAsync(ApplicationDto application, VerificationPolicyDto policy)
        {
            var taskVariants = await GetTaskVariantsAsync(application, policy);
            var (requestStepCommands, collectionStepIds) = await BuildCollectionSteps(application.UserId, taskVariants);
            var (createTaskCommands, taskIds) = BuildTasks(application.UserId, taskVariants, collectionStepIds);
            var (requestCheckCommands, providers) = await BuildChecksAsync(application.UserId, taskVariants, taskIds);
            var externalProfileCommands = await BuildExternalProfilesAsync(application.UserId, providers);

            var addRequiredTasksCommand = new AddRequiredTasksCommand(
                application.UserId,
                application.Id,
                taskIds.Values.ToArray(),
                InitiationReasons.AddRequiredTasksByPolicy);

            var automateApplicationCommand = new AutomateApplicationCommand(
                application.UserId,
                application.Id,
                InitiationReasons.ApplicationIsBuilt);

            var commands = requestStepCommands
                           .Cast<VerificationCommand>()
                           .Concat(createTaskCommands)
                           .Concat(requestCheckCommands)
                           .Concat(externalProfileCommands)
                           .Concat(new[] { addRequiredTasksCommand })
                           .Concat(new[] { automateApplicationCommand })
                           .ToArray();

            await _commandsPublisher.PublishAsync(commands);
        }

        private async Task<TaskVariant[]> GetTaskVariantsAsync(ApplicationDto application, VerificationPolicyDto policy)
        {
            var existingTasks = application.Tasks.ToDictionary(task => task.VariantId);
            var taskVariants = policy.Tasks.Where(variant => !existingTasks.ContainsKey(variant.VariantId)).ToArray();

            var checkVariantIds = taskVariants.SelectMany(variant => variant.CheckVariants).ToArray();
            var checkCollectionSteps = await GetCheckCollectionStepsAsync(checkVariantIds);

            return taskVariants.Select(variant =>
            {
                var checkSteps = variant.CheckVariants
                                   .GroupJoin(checkCollectionSteps,
                                       variantId => variantId,
                                       step => step.VariantId,
                                       (_, steps) => steps)
                                   .SelectMany(step => step);

                return TaskVariant.Create(variant).With(checkSteps);
            }).ToArray();
        }

        private async Task<CheckCollectionStep[]> GetCheckCollectionStepsAsync(IEnumerable<Guid> checkVariantIds)
        {
            var getCheckParameters = checkVariantIds.Select(GetCheckParametersAsync);
            var checkParameters = await Task.WhenAll(getCheckParameters);

            return checkParameters
                   .SelectMany(
                       tuple => tuple.Parameters,
                       (tuple, data) => new CheckCollectionStep
                       {
                           VariantId = tuple.VariantId,
                           XPath = data.XPath,
                           IsRequired = data.IsRequired,
                           IsReviewNeeded = false
                       }).ToArray();

            async Task<(Guid VariantId, CheckInputParameterDto[] Parameters)> GetCheckParametersAsync(Guid variantId)
            {
                var parameters = await _checkProviderService.GetParametersAsync(variantId);
                return (variantId, parameters);
            }
        }

        private async Task<(CreateCollectionStepCommand[], IDictionary<string, Guid>)> BuildCollectionSteps(Guid userId, TaskVariant[] taskVariants)
        {
            var collectionSteps = taskVariants
                                  .SelectMany(variant => variant.GetCollectionSteps())
                                  .GroupBy(step => step.XPath, MergeCollectionSteps)
                                  .ToArray();

            var policyXPathes = collectionSteps.Select(step => step.XPath).ToArray();
            var submittedSteps = await SelectSubmittedSteps(userId, policyXPathes);

            var commands = collectionSteps.Select(step =>
            {
                var stepId = _idGenerator.Generate(HashCode.Combine(userId, step.XPath));
                var isSubmitted = submittedSteps.Contains(step.XPath);
                return new CreateCollectionStepCommand(
                    userId,
                    stepId,
                    step.XPath,
                    step.IsRequired,
                    step.IsReviewNeeded,
                    isSubmitted,
                    InitiationReasons.RequestCollectionStepByPolicy);
            }).ToArray();

            var collectionStepIds = commands.ToDictionary(
                command => command.XPath,
                command => command.CommandId);

            return (commands, collectionStepIds);

            static PolicyCollectionStep MergeCollectionSteps(string xPath, IEnumerable<PolicyCollectionStep> steps) =>
                steps.Aggregate((mergedStep, step) =>
                {
                    mergedStep.IsRequired |= step.IsRequired;
                    mergedStep.IsReviewNeeded |= step.IsReviewNeeded;
                    return mergedStep;
                });
        }

        private (CreateTaskCommand[], IDictionary<Guid, Guid>) BuildTasks(Guid userId, IEnumerable<TaskVariant> taskVariants, IDictionary<string, Guid> collectionStepsMap)
        {
            var commands = taskVariants.Select(variant =>
            {
                var taskId = _idGenerator.Generate(HashCode.Combine(userId, variant.Id));
                var collectionStepIds = variant.GetCollectionSteps()
                                        .Select(step => collectionStepsMap[step.XPath])
                                        .ToArray();

                return new CreateTaskCommand(
                    userId,
                    taskId,
                    variant.Type.ToString(),
                    variant.Id,
                    variant.CheckVariants,
                    collectionStepIds,
                    InitiationReasons.CreateTaskByPolicy);
            }).ToArray();

            var taskIds = commands.ToDictionary(
                command => command.VariantId,
                command => command.CommandId);

            return (commands, taskIds);
        }

        private async Task<(RequestCheckCommand[], CheckProviderType[])> BuildChecksAsync(Guid userId, TaskVariant[] taskVariants, IDictionary<Guid, Guid> tasksMap)
        {
            var acceptanceChecks = taskVariants
                           .SelectMany(
                               taskVariant => taskVariant.CheckVariants,
                               (taskVariant, checkVariantId) => new
                               {
                                   CheckVariantId = checkVariantId,
                                   RelatedTask = tasksMap[taskVariant.Id]
                               })
                           .GroupBy(
                               variant => variant.CheckVariantId,
                               (checkVariantId, grouped) =>
                               {
                                   var relatedTasks = grouped.Select(x => x.RelatedTask);
                                   return new { VariantId = checkVariantId, RelatedTasks = relatedTasks.ToArray() };
                               })
                           .ToArray();

            var variantIds = acceptanceChecks.Select(check => check.VariantId).ToArray();
            var checkVariants = await _policyStorage.GetChecksInfoAsync(variantIds);
            var checkVariantsMap = checkVariants.ToDictionary(variant => variant.Id);

            var commands = acceptanceChecks.Select(check =>
            {
                var checkId = _idGenerator.Generate(HashCode.Combine(userId, check.VariantId));
                var checkVariant = checkVariantsMap[check.VariantId];
                return new RequestCheckCommand(
                    userId,
                    checkId,
                    checkVariant.Id,
                    checkVariant.Type.ToString(),
                    checkVariant.Provider.ToString(),
                    check.RelatedTasks,
                    InitiationReasons.RequestCheckByPolicy);
            }).ToArray();

            var checkProviders = checkVariants
                                 .Select(variant => variant.Provider)
                                 .ToArray();

            return (commands, checkProviders);
        }

        private async Task<CreateExternalProfileCommand[]> BuildExternalProfilesAsync(Guid userId, CheckProviderType[] checkProviders)
        {
            var externalProviders = checkProviders
                                        .Select(_externalProviderTypeMapper.Map)
                                        .Where(type => type != null)
                                        .ToArray();

            if (externalProviders.Length == 0)
                return Array.Empty<CreateExternalProfileCommand>();

            var existingProfiles = await _externalProfileStorage.FindAsync(userId);
            var existingProviders = existingProfiles.Select(profile => (ExternalProviderType?)profile.Provider);
            return externalProviders
                   .Except(existingProviders)
                   .Select(provider => new CreateExternalProfileCommand(userId, provider.ToString()))
                   .ToArray();
        }

        private async Task<string[]> SelectSubmittedSteps(Guid userId, string[] xPathes)
        {
            xPathes = xPathes.Where(IsNotSurveyStep)
                             .Where(IsNotDocumentStep)
                             .ToArray();

            var profileDataProvider = _profileProviderFactory.Create(userId);
            var profileData = await profileDataProvider.ReadAsync(xPathes);

            return xPathes
                   .GroupJoin(profileData,
                       xPath => xPath,
                       data => data.Key,
                       (step, data) => new { Step = step, IsSubmitted = data.Any() })
                   .Where(x => x.IsSubmitted)
                   .Select(x => x.Step)
                   .ToArray();

            bool IsNotDocumentStep(string xPath) => _xPathParser.Parse(xPath) is not DocumentsXPathDetails;
            bool IsNotSurveyStep(string xPath) => _xPathParser.Parse(xPath) is not SurveyXPathDetails;
        }

        private class CheckCollectionStep : PolicyCollectionStep
        {
            public Guid VariantId { get; set; }
        }

        private class TaskVariant
        {
            public Guid Id { get; private set; }

            public TaskType Type { get; private set; }

            public Guid[] CheckVariants { get; private set; }

            public PolicyCollectionStep[] TaskSteps { get; private set; }

            public PolicyCollectionStep[] CheckSteps { get; private set; }

            public static TaskVariant Create(TaskVariantDto variant) => new()
            {
                Id = variant.VariantId,
                Type = variant.Type,
                CheckVariants = variant.CheckVariants,
                TaskSteps = variant.CollectionSteps,
                CheckSteps = Array.Empty<PolicyCollectionStep>()
            };

            public TaskVariant With(IEnumerable<PolicyCollectionStep> checkSteps)
            {
                if (checkSteps == null)
                    throw new ArgumentNullException(nameof(checkSteps));

                CheckSteps = checkSteps.ToArray();
                return this;
            }

            public IEnumerable<PolicyCollectionStep> GetCollectionSteps() => TaskSteps.Concat(CheckSteps);
        }
    }
}