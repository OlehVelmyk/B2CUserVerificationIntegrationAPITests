using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Services.Mappers;
using WX.B2C.User.Verification.Core.Services.Utilities;
using WX.B2C.User.Verification.Domain;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Core.Services
{
    internal class TaskService : ITaskService
    {
        private readonly ICollectionStepRepository _stepRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ISystemClock _systemClock;
        private readonly IInitiationMapper _initiationMapper;
        private readonly IEventPublisher _eventPublisher;

        public TaskService(
            ICollectionStepRepository stepRepository,
            ITaskRepository taskRepository, 
            ISystemClock systemClock,
            IInitiationMapper initiationMapper,
            IEventPublisher eventPublisher)
        {
            _stepRepository = stepRepository ?? throw new ArgumentNullException(nameof(stepRepository));
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            _initiationMapper = initiationMapper ?? throw new ArgumentNullException(nameof(initiationMapper));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public Task<Guid> CreateAsync(NewTaskDto newTaskDto, InitiationDto initiationDto) =>
            CreateAsync(async () =>
                {
                    var collectionSteps = newTaskDto.CollectionStepIds.IsNullOrEmpty()
                        ? Array.Empty<CollectionStep>()
                        : await newTaskDto.CollectionStepIds.Foreach(_stepRepository.GetAsync);

                    var taskId = newTaskDto.Id ?? Guid.NewGuid();
                    var creationDate = _systemClock.GetDate();
                    return VerificationTask.Create(taskId,
                        newTaskDto.UserId,
                        newTaskDto.Type,
                        newTaskDto.VariantId,
                        creationDate,
                        collectionSteps,
                        _initiationMapper.Map(initiationDto));
                })
                .Select(x => x.Id);

        public Task AddCollectionStepsAsync(Guid taskId, Guid[] collectionStepIds, InitiationDto initiationDto) =>
            UpdateAsync(taskId, async task =>
            {
                var initiation = _initiationMapper.Map(initiationDto);
                foreach (var collectionStepId in collectionStepIds)
                {
                    var collectionStep = await _stepRepository.GetAsync(collectionStepId);
                    task.AddCollectionStep(collectionStep, initiation);
                }
            });

        public Task RemoveCollectionStepAsync(Guid taskId, Guid collectionStepId) =>
            UpdateAsync(taskId, task => task.RemoveCollectionStep(collectionStepId));

        public Task CompleteAsync(Guid taskId, TaskResult result, InitiationDto initiationDto) =>
            UpdateAsync(taskId, task =>
            {
                var initiation = _initiationMapper.Map(initiationDto);
                task.Complete(result, initiation);
            });

        public Task IncompleteAsync(Guid taskId, InitiationDto initiationDto) =>
            UpdateAsync(taskId, task =>
            {
                var initiation = _initiationMapper.Map(initiationDto);
                task.Incomplete(initiation);
            });

        private Task<VerificationTask> CreateAsync(Func<Task<VerificationTask>> create)
        {
            if (create == null)
                throw new ArgumentNullException(nameof(create));

            return AppCore.ApplyChangesAsync(create, SaveAndPublishAsync);
        }

        private Task<VerificationTask> UpdateAsync(Guid taskId, Action<VerificationTask> update) =>
            UpdateAsync(taskId, update?.ToAsync());

        private Task<VerificationTask> UpdateAsync(Guid taskId, Func<VerificationTask, Task> update)
        {
            if (update == null)
                throw new ArgumentNullException(nameof(update));

            return AppCore.ApplyChangesAsync(
                () => _taskRepository.GetAsync(taskId),
                update,
                SaveAndPublishAsync);
        }

        private async Task SaveAndPublishAsync(VerificationTask task)
        {
            await _taskRepository.SaveAsync(task);
            await _eventPublisher.PublishAsync(task.CommitEvents());
        }
    }
}
