using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.DataCollection;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Services
{
    public interface ICollectionStepAggregationService
    {
        Task<CollectionStepDto> AggregateAsync(Core.Contracts.Dtos.CollectionStepDto collectionStep);

        Task<IEnumerable<CollectionStepDto>> AggregateAsync(Core.Contracts.Dtos.CollectionStepDto[] collectionSteps);
    }

    internal class CollectionStepAggregationService : ICollectionStepAggregationService
    {
        private readonly IProfileProviderFactory _profileProviderFactory;
        private readonly ICollectionStepStorage _collectionStepStorage;
        private readonly ICollectionStepMapper _mapper;

        public CollectionStepAggregationService(
            IProfileProviderFactory profileProviderFactory, 
            ICollectionStepStorage collectionStepStorage,
            ICollectionStepMapper mapper)
        {
            _profileProviderFactory = profileProviderFactory ?? throw new ArgumentNullException(nameof(profileProviderFactory));
            _collectionStepStorage = collectionStepStorage ?? throw new ArgumentNullException(nameof(collectionStepStorage));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CollectionStepDto> AggregateAsync(Core.Contracts.Dtos.CollectionStepDto collectionStep)
        {
            if (collectionStep == null)
                throw new ArgumentNullException(nameof(collectionStep));

            var result = await AggregateAsync(new[] { collectionStep });
            return result.Single();
        }

        public async Task<IEnumerable<CollectionStepDto>> AggregateAsync(Core.Contracts.Dtos.CollectionStepDto[] collectionSteps)
        {
            if (collectionSteps == null)
                throw new ArgumentNullException(nameof(collectionSteps));
            if (!collectionSteps.Any())
                return Enumerable.Empty<CollectionStepDto>();

            var profileData = await GetProfileDataAsync(collectionSteps);
            var relatedTasksMap = await GetRelatedTasksAsync(collectionSteps);
            return collectionSteps.Select(step => Map(step, profileData, relatedTasksMap))
                                  .OrderByDescending(step => step.RequestedAt);
        }

        private async Task<IProfileDataCollection> GetProfileDataAsync(Core.Contracts.Dtos.CollectionStepDto[] collectionSteps)
        {
            var userId = collectionSteps.Select(x => x.UserId).First();

            var xPathesToRead = collectionSteps
                                .Where(IsSubmitted)
                                .Select(step => step.XPath)
                                .Distinct()
                                .ToArray();

            var provider = _profileProviderFactory.Create(userId);
            return await provider.ReadAsync(xPathesToRead);

            static bool IsSubmitted(Core.Contracts.Dtos.CollectionStepDto step) =>
                step.State > CollectionStepState.Requested;
        }

        private async Task<Dictionary<Guid, Guid[]>> GetRelatedTasksAsync(IEnumerable<Core.Contracts.Dtos.CollectionStepDto> collectionSteps)
        {
            var stepIds = collectionSteps.Select(step => step.Id);
            var relatedTasks = await stepIds.Select(GetRelatedTasks).WhenAll();

            return relatedTasks.ToDictionary(
                step => step.Id,
                step => step.RelatedTasks);

            Task<(Guid Id, Guid[] RelatedTasks)> GetRelatedTasks(Guid stepId) =>
                _collectionStepStorage.GetRelatedTasksAsync(stepId)
                             .Select(taskIds => (stepId, taskIds));
        }

        private CollectionStepDto Map(Core.Contracts.Dtos.CollectionStepDto collectionStep, 
                                      IProfileDataCollection profileData, 
                                      IDictionary<Guid, Guid[]> relatedTasksMap)
        {
            var collectedData = profileData.ValueOrNull(collectionStep.XPath);
            var relatedTasks = relatedTasksMap[collectionStep.Id];
            return _mapper.Map(collectionStep, collectedData, relatedTasks);
        }
    }
}
