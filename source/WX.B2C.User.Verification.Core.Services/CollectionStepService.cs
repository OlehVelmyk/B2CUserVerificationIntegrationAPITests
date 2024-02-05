using System;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Core.Services.Mappers;
using WX.B2C.User.Verification.Core.Services.Utilities;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Domain.Shared;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Core.Services
{
    internal class CollectionStepService : ICollectionStepService
    {
        private readonly ITaskStorage _taskStorage;
        private readonly ICollectionStepRepository _stepRepository;
        private readonly IInitiationMapper _initiationMapper;
        private readonly IXPathParser _xPathParser;
        private readonly IEventPublisher _eventPublisher;

        public CollectionStepService(
            ITaskStorage taskStorage,
            ICollectionStepRepository stepRepository,
            IInitiationMapper initiationMapper,
            IXPathParser xPathParser,
            IEventPublisher eventPublisher)
        {
            _taskStorage = taskStorage ?? throw new ArgumentNullException(nameof(taskStorage));
            _stepRepository = stepRepository ?? throw new ArgumentNullException(nameof(stepRepository));
            _initiationMapper = initiationMapper ?? throw new ArgumentNullException(nameof(initiationMapper));
            _xPathParser = xPathParser ?? throw new ArgumentNullException(nameof(xPathParser));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        public Task<Guid> RequestAsync(Guid userId, NewCollectionStepDto newStepDto, InitiationDto initiationDto)
        {
            if (!_xPathParser.IsValid(newStepDto.XPath))
                throw new InvalidOperationException($"XPath {newStepDto.XPath} is not valid");

            var initiation = _initiationMapper.Map(initiationDto);

            var collectionStepId = newStepDto.Id ?? Guid.NewGuid();
            return CreateOrUpdateAsync(
                    () => _stepRepository.FindNotCompletedAsync(userId, newStepDto.XPath),
                    () => CollectionStep.Request(
                        collectionStepId, 
                        userId,
                        newStepDto.XPath,
                        newStepDto.IsRequired,
                        newStepDto.IsReviewNeeded,
                        initiation),
                    step =>
                    {
                        if (newStepDto.IsRequired && !step.IsRequired)
                            step.Require(initiation);

                        if (newStepDto.IsReviewNeeded && !step.IsReviewNeeded)
                            step.RequireReview(initiation);
                    })
                .Select(x => x.Id);
        }

        public Task SubmitAsync(Guid stepId, InitiationDto initiationDto) =>
            UpdateAsync(stepId, step =>
            {
                var initiation = _initiationMapper.Map(initiationDto);

                if (step.IsReviewNeeded)
                    step.MoveIntoReview(initiation);
                else
                    step.Complete(initiation);
            });

        public Task ReviewAsync(Guid stepId, CollectionStepReviewResult reviewResult, InitiationDto initiationDto) =>
            UpdateAsync(stepId, step =>
            {
                var initiation = _initiationMapper.Map(initiationDto);
                step.Review(reviewResult, initiation);
            });

        public async Task RemoveAsync(Guid stepId, InitiationDto initiationDto)
        {
            var tasks = await _taskStorage.FindByStepIdAsync(stepId);
            if (tasks.Any())
                throw new InvalidOperationException("Collection step should not contains assigned tasks");

            await _stepRepository.RemoveAsync(stepId);
        }

        public Task UpdateAsync(Guid stepId, CollectionStepPatch collectionStepPatch, InitiationDto initiationDto) =>
            UpdateAsync(stepId,
                step =>
                {
                    var initiation = _initiationMapper.Map(initiationDto);
                    step.Update(collectionStepPatch.IsRequired, collectionStepPatch.IsReviewNeeded, initiation);
                });

        public Task CancelAsync(Guid stepId, InitiationDto initiationDto) =>
            UpdateAsync(stepId,
                step =>
                {
                    var initiation = _initiationMapper.Map(initiationDto);
                    step.Cancel(initiation);
                });

        private Task<CollectionStep> CreateOrUpdateAsync(
            Func<Task<CollectionStep>> find,
            Func<CollectionStep> create,
            Action<CollectionStep> update = null)
        {
            if (find == null)
                throw new ArgumentNullException(nameof(find));
            if (create == null)
                throw new ArgumentNullException(nameof(create));

            return AppCore.ApplyChangesAsync(async () =>
            {
                var collectionStep = await find();
                collectionStep ??= create.Invoke();
                return collectionStep;
            }, update, SaveAndPublishAsync);
        }

        private Task<CollectionStep> UpdateAsync(Guid stepId, Action<CollectionStep> update)
        {
            if (update == null)
                throw new ArgumentNullException(nameof(update));

            return AppCore.ApplyChangesAsync(
                () => _stepRepository.GetAsync(stepId),
                update,
                SaveAndPublishAsync);
        }

        private async Task SaveAndPublishAsync(CollectionStep step)
        {
            await _stepRepository.SaveAsync(step);
            await _eventPublisher.PublishAsync(step.CommitEvents());
        }
    }
}