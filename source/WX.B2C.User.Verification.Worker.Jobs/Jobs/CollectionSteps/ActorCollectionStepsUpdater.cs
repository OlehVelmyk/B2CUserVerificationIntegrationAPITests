using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Worker.Jobs.DataAccess.Repositories;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps
{
    internal class ActorCollectionStepsUpdater : ICollectionStepsUpdater
    {
        private readonly ICollectionStepService _collectionStepService;
        private readonly string _jobName;
        public ActorCollectionStepsUpdater(ICollectionStepService collectionStepService, string jobName)
        {
            _collectionStepService = collectionStepService ?? throw new ArgumentNullException(nameof(collectionStepService));
            _jobName = jobName ?? throw new ArgumentNullException(nameof(jobName));
        }

        public async Task<Dictionary<string,Guid>> SaveAsync(Guid userId, 
                                                             Dictionary<PolicyCollectionStep, NewCollectionStep> collectionSteps, 
                                                             CollectionStepEntity[] existingSteps)
        {
            var initiation = InitiationDto.Create("Job", _jobName);
            var steps = await collectionSteps.Foreach(pair =>
            {
                return SaveCollectionStepByActorAsync(userId,
                                                      pair.Key,
                                                      pair.Value,
                                                      existingSteps.FirstOrDefault(entity => entity.XPath == pair.Key.XPath),
                                                      initiation);
            });

            return steps.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }

        private async Task<(string,Guid)> SaveCollectionStepByActorAsync(Guid userId,
                                                                         PolicyCollectionStep step,
                                                                         NewCollectionStep newCollectionStep,
                                                                         CollectionStepEntity persistedStep, 
                                                                         InitiationDto initiation)
        {
            if (persistedStep == null || MustBeRequestedAgain(persistedStep, newCollectionStep))
                persistedStep = await RequestStepAsync(userId, step, newCollectionStep, initiation);

            await SubmitStepAsync(newCollectionStep, step, persistedStep, initiation);
            await ReviewStepAsync(newCollectionStep, step, persistedStep, initiation);
            return (persistedStep.XPath, persistedStep.Id);

            bool MustBeRequestedAgain(CollectionStepEntity persistedStep, NewCollectionStep expected) =>
                !persistedStep.IsRequired && expected.IsRequired && persistedStep.State == CollectionStepState.Requested;
        }

        private async Task<CollectionStepEntity> RequestStepAsync(Guid userId,
                                                                  PolicyCollectionStep step,
                                                                  NewCollectionStep expected,
                                                                  InitiationDto initiation)
        {
            var newCollectionStepDto = new NewCollectionStepDto
            {
                XPath = step.XPath,
                IsRequired = step.IsRequired || expected.IsRequired,
                IsReviewNeeded = step.IsReviewNeeded
            };

            var stepId = await _collectionStepService.RequestAsync(userId, newCollectionStepDto, initiation);
            var persistedStep = new CollectionStepEntity
            {
                Id = stepId,
                State = CollectionStepState.Requested
            };
            return persistedStep;
        }

        private async Task SubmitStepAsync(NewCollectionStep expected,
                                           PolicyCollectionStep step,
                                           CollectionStepEntity persistedStep,
                                           InitiationDto initiation)
        {
            if (expected.State == CollectionStepState.Requested || persistedStep.State != CollectionStepState.Requested)
                return;

            await _collectionStepService.SubmitAsync(persistedStep.Id, initiation);
            persistedStep.State = step.IsReviewNeeded ? CollectionStepState.InReview : CollectionStepState.Completed;
        }

        private async Task ReviewStepAsync(NewCollectionStep expected,
                                           PolicyCollectionStep step,
                                           CollectionStepEntity persistedStep,
                                           InitiationDto initiation)
        {
            if (!step.IsReviewNeeded)
                return;

            if (expected.State == CollectionStepState.Completed && persistedStep.State == CollectionStepState.InReview)
                await _collectionStepService.ReviewAsync(persistedStep.Id, expected.Result.Value, initiation);
        }
    }
}