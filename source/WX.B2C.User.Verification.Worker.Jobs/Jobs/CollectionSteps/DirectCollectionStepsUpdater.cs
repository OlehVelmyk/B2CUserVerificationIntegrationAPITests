using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;
using ICollectionStepRepository = WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories.ICollectionStepRepository;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps
{
    internal class DirectCollectionStepsUpdater : ICollectionStepsUpdater
    {
        private readonly ICollectionStepRepository _collectionStepRepository;

        public DirectCollectionStepsUpdater(ICollectionStepRepository collectionStepRepository)
        {
            _collectionStepRepository = collectionStepRepository ?? throw new ArgumentNullException(nameof(collectionStepRepository));
        }

        public async Task<Dictionary<string, Guid>> SaveAsync(Guid userId,
                                                              Dictionary<PolicyCollectionStep, NewCollectionStep> collectionSteps,
                                                              CollectionStepEntity[] existingSteps)
        {
            var steps = collectionSteps.Select(pair =>
                                       {
                                           return PrepareCollectionStep(userId,
                                                                        pair.Key,
                                                                        pair.Value,
                                                                        existingSteps.FirstOrDefault(entity =>
                                                                            entity.XPath == pair.Key.XPath));
                                       })
                                       .ToArray();

            var stepsToCreate = steps.Where(step => step.Action == RequiredAction.Create).Select(tuple => tuple.Step).ToArray();
            var stepsToUpdate = steps.Where(step => step.Action == RequiredAction.Update).Select(tuple => tuple.Step).ToArray();

            await _collectionStepRepository.CreateAsync(stepsToCreate);
            await _collectionStepRepository.UpdateAsync(stepsToUpdate);

            return steps.Where(tuple => tuple.Action != RequiredAction.None)
                        .ToDictionary(tuple => tuple.Step.XPath, tuple => tuple.Step.Id);
        }

        private (CollectionStepEntity Step, RequiredAction Action) PrepareCollectionStep(Guid userId,
                                                                                         PolicyCollectionStep step,
                                                                                         NewCollectionStep expected,
                                                                                         CollectionStepEntity persistedStep)
        {
            // TODO Open question if existing step is in review or even complete
            // TODO For now we just update such step directly
            var isNew = persistedStep == null;

            var expectedState = expected.State;
            // If in new verification was decided to decline review for some collection steps (like SOF survey)
            // Then we automatically approve such survey after migration even if it was not approved by compliance
            if (expectedState == CollectionStepState.InReview && !step.IsReviewNeeded)
                expectedState = CollectionStepState.Completed;

            var newCollectionStep = new CollectionStepEntity
            {
                UserId = userId,
                Id = isNew ? Guid.NewGuid() : persistedStep.Id,
                CreatedAt = isNew ? DateTime.UtcNow : persistedStep.CreatedAt,
                UpdatedAt = isNew ? null : DateTime.UtcNow,
                IsRequired = step.IsRequired || expected.IsRequired,
                ReviewResult = step.IsReviewNeeded ? expected.Result : null,
                State = expectedState,
                XPath = step.XPath,
                IsReviewNeeded = step.IsReviewNeeded
            };
            var hasChangesToSave = isNew || HasChangesToSave(newCollectionStep, persistedStep);

            var action = isNew ? RequiredAction.Create : hasChangesToSave ? RequiredAction.Update : RequiredAction.None;

            return (newCollectionStep, action);
        }

        private bool HasChangesToSave(CollectionStepEntity newCollectionStep, CollectionStepEntity persistedStep) =>
            newCollectionStep.State != persistedStep.State
         || newCollectionStep.IsRequired != persistedStep.IsRequired
         || newCollectionStep.ReviewResult != persistedStep.ReviewResult;

        private enum RequiredAction
        {
            None,
            Create,
            Update
        }
    }
}