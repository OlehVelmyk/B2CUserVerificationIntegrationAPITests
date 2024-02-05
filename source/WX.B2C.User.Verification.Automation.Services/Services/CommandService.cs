using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WX.B2C.User.Verification.Core.Contracts.Commands;
using WX.B2C.User.Verification.Core.Contracts.Commands.ServiceClients;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Exceptions;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Automation.Services.Commands
{
    internal class CommandService : ICommandService
    {
        private readonly ITaskServiceClient _taskService;
        private readonly ICollectionStepServiceClient _collectionStepService;
        private readonly IApplicationServiceClient _applicationService;
        private readonly ICheckServiceClient _checkService;

        private readonly IVerificationPolicyStorage _policyStorage;
        private readonly ITaskStorage _taskStorage;
        private readonly ICheckStorage _checkStorage;
        private readonly IIdempotentGuidGenerator _idempotentGuidGenerator;
        private readonly ICollectionStepStorage _collectionStepStorage;
        private readonly ILogger _logger;

        public CommandService(ITaskServiceClient taskService,
                              ICollectionStepServiceClient collectionStepService,
                              IApplicationServiceClient applicationService,
                              ICheckServiceClient checkService,
                              IVerificationPolicyStorage policyStorage,
                              ITaskStorage taskStorage,
                              ICheckStorage checkStorage,
                              IIdempotentGuidGenerator idempotentGuidGenerator,
                              ICollectionStepStorage collectionStepStorage,
                              ILogger logger)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _collectionStepService = collectionStepService ?? throw new ArgumentNullException(nameof(collectionStepService));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _checkService = checkService ?? throw new ArgumentNullException(nameof(checkService));
            _policyStorage = policyStorage ?? throw new ArgumentNullException(nameof(policyStorage));
            _taskStorage = taskStorage ?? throw new ArgumentNullException(nameof(taskStorage));
            _checkStorage = checkStorage ?? throw new ArgumentNullException(nameof(checkStorage));
            _idempotentGuidGenerator = idempotentGuidGenerator ?? throw new ArgumentNullException(nameof(idempotentGuidGenerator));
            _collectionStepStorage = collectionStepStorage ?? throw new ArgumentNullException(nameof(collectionStepStorage));
            _logger = logger?.ForContext<CommandService>() ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RunAsync(AddTaskCommand command, InitiationDto initiationDto)
        {
            var taskVariant = await _policyStorage.GetTaskVariantAsync(command.VariantId);
            var existingTask = await _taskStorage.FindAsync(command.ApplicationId, taskVariant.Type);
            if (existingTask != null)
                return;

            var newTaskDto = new NewTaskDto
            {
                UserId = command.UserId,
                Type = taskVariant.Type,
                VariantId = taskVariant.VariantId,
                //TODO we need to add collection steps and collection steps from checks as well
                //TODO Think about making creation task decoupled from creating collection steps
                //TODO For this, we need to be sure that task will not be completed until we add all collection steps
                AcceptanceCheckIds = taskVariant.CheckVariants,
            };

            var taskId = await _taskService.CreateAsync(newTaskDto, initiationDto);

            if (command.AddCompleted)
                await _taskService.CompleteAsync(taskId, TaskResult.Passed, initiationDto);

            // Due to race condition we cannot rely that if _taskStorage.FindTaskAsync returns null
            // that in the next moment the same task will be added.
            try
            {
                await _applicationService.AddRequiredTasksAsync(command.ApplicationId, new[] { taskId }, initiationDto);
            }
            catch (ApplicationTaskAlreadyExistsException)
            {
                // TODO Must be fixed in WRXB-9854
                _logger.Warning("Rudiment task created TaskId {TaskId}", taskId);
                // TODO later we can remove useless task. For now we do not have such method
            }
        }

        public async Task RunAsync(AddCollectionStepsToTaskCommand command, InitiationDto initiationDto)
        {
            var task = await _taskStorage.GetAsync(command.ApplicationId, command.TaskType);
            await _taskService.AddCollectionStepsAsync(task.Id, command.CollectionSteps, initiationDto);
        }

        public async Task<Guid> RunAsync(AddCollectionStepCommand command, InitiationDto initiationDto)
        {
            if (!command.ForceCreating)
            {
                var collectionStep = await _collectionStepStorage.FindAsync(command.UserId, command.CollectionStep.XPath);
                if (collectionStep is { State: not CollectionStepState.Cancelled } && !IsStepNeedToBeUpdated(collectionStep))
                    return collectionStep.Id;
            }

            var newCollectionStepDto = new NewCollectionStepDto
            {
                //TODO open question: if data exists and collection step also created (collection step which need review)
                //TODO what do we need to do? For now we will not create new step and return existing
                IsRequired = command.CollectionStep.IsRequired,
                IsReviewNeeded = command.CollectionStep.IsReviewNeeded,
                XPath = command.CollectionStep.XPath
            };

            //TODO https://wirexapp.atlassian.net/browse/WRXB-10877
            return await _collectionStepService.RequestAsync(command.UserId, newCollectionStepDto, initiationDto);

            bool IsStepNeedToBeUpdated(CollectionStepDto collectionStep) =>
                collectionStep.State == CollectionStepState.Requested &&
                ((!collectionStep.IsRequired && command.CollectionStep.IsRequired) ||
                 (!collectionStep.IsReviewNeeded && command.CollectionStep.IsReviewNeeded));
        }

        public async Task RunAsync(InstructCheckCommand command, InitiationDto initiationDto)
        {
            // TODO What if two triggers will have force mode and fired at the same time.
            // Then we still need to check if check already created (by id which created in idempotent way) or no.
            // Not real case for now, as only one check can be created in policies
            // Must be fixed in WRXB-9854

            if (!command.Force)
            {
                // TODO What if check was added to another application, and we create new one during changing country
                // Not real case for now, as only one check can be created in policies
                var existingChecks = await _checkStorage.GetAsync(command.UserId, new[] { command.VariantId });
                if (existingChecks.Any())
                    return;
            }

            var checkInfo = await _policyStorage.FindCheckInfoAsync(command.VariantId);
            var task = await _taskStorage.GetAsync(command.ApplicationId, command.TaskType);
            var newCheckId = _idempotentGuidGenerator.Generate(HashCode.Combine(command.VariantId, command.ApplicationId));

            var newCheckDto = new NewCheckDto
            {
                Id = newCheckId,
                Provider = checkInfo.Provider,
                CheckType = checkInfo.Type,
                VariantId = command.VariantId,
                RelatedTasks = new[] { task.Id }
            };

            try
            {
                await _checkService.RequestAsync(command.UserId, newCheckDto, initiationDto);
            }
            catch (Exception e)
            {
                // Due to race condition if two triggers if two triggers tries to add the same check variant. Must be fixed in WRXB-9854 
                // Not real case for now, as only one check can be created in policies
                _logger.Error(e,
                              "New check is not instructed due to race condition. CheckVariant:{VariantId}, UserId: {UserId}",
                              command.VariantId,
                              command.UserId);
            }
        }
    }
}