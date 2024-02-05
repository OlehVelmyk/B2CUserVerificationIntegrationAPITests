using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Extensions;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers;
using Tasks = WX.B2C.User.Verification.Extensions.TaskExtensions;
using OutputTaskDto = WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos.TaskDto;
using OutputTaskCheckDto = WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos.TaskCheckDto;
using OutputCheckVariantDto = WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos.CheckVariantDto;
using OutputTaskVariantDto = WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos.TaskVariantDto;
using OutputTaskCollectionStepDto = WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos.TaskCollectionStepDto;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Services
{
    public interface ITaskAggregationService
    {
        Task<OutputTaskDto> AggregateAsync(TaskDto task);

        Task<IEnumerable<OutputTaskDto>> AggregateAsync(TaskDto[] tasks);
    }

    public class TaskAggregationService : ITaskAggregationService
    {
        private readonly ICheckStorage _checkStorage;
        private readonly IVerificationPolicyStorage _verificationPolicyStorage;
        private readonly IProfileProviderFactory _profileProviderFactory;
        private readonly ICollectionStepBriefDataMapper _collectionStepBriefDataMapper;
        private readonly ITaskMapper _mapper;

        public TaskAggregationService(
            ICheckStorage checkStorage,
            IVerificationPolicyStorage verificationPolicyStorage,
            IProfileProviderFactory profileProviderFactory,
            ICollectionStepBriefDataMapper collectionStepBriefDataMapper,
            ITaskMapper mapper)
        {
            _checkStorage = checkStorage ?? throw new ArgumentNullException(nameof(checkStorage));
            _verificationPolicyStorage = verificationPolicyStorage ?? throw new ArgumentNullException(nameof(verificationPolicyStorage));
            _profileProviderFactory = profileProviderFactory ?? throw new ArgumentNullException(nameof(profileProviderFactory));
            _collectionStepBriefDataMapper = collectionStepBriefDataMapper ?? throw new ArgumentNullException(nameof(collectionStepBriefDataMapper));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<OutputTaskDto> AggregateAsync(TaskDto task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            var result = await AggregateAsync(new[] { task });
            return result.Single();
        }

        public async Task<IEnumerable<OutputTaskDto>> AggregateAsync(TaskDto[] tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));
            if (tasks.IsEmpty())
                return Enumerable.Empty<OutputTaskDto>();

            var taskVariants = await GetTaskVariantsAsync(tasks);
            var profileData = await GetProfileDataAsync(tasks);
            var checkVariantNames = await GetCheckVariantsAsync(tasks);
            var collectionStepsFormattedData = await GetCollectionStepsFormattedDataAsync(profileData);
            
            return tasks.Select(task => Map(task, taskVariants, checkVariantNames, collectionStepsFormattedData)).ToArray();
        }

        private async Task<IReadOnlyDictionary<string, string>> GetCollectionStepsFormattedDataAsync(IProfileDataCollection profileData)
        {
            var profileDataDictionary = profileData.ToDictionary(pair => pair.Key, pair => pair.Value);
            var collectionStepsMapped = await _collectionStepBriefDataMapper.MapAsync(profileDataDictionary);
            return collectionStepsMapped.ToDictionary(pair => pair.Key, pair => pair.Value.Value);
        }

        private Task<TaskVariantDto[]> GetTaskVariantsAsync(IEnumerable<TaskDto> tasks)
        {
            var taskVariantIds = tasks.Select(c => c.VariantId)
                                       .Distinct()
                                       .ToArray();

            return _verificationPolicyStorage.GetTaskVariantsAsync(taskVariantIds);
        }

        private async Task<IProfileDataCollection> GetProfileDataAsync(TaskDto[] tasks)
        {
            var userId = tasks.Select(x => x.UserId).First();
            var xPathesToRead = tasks.SelectMany(x => x.CollectionSteps)
                                     .Where(step => step.State == Domain.DataCollection.CollectionStepState.Completed)
                                     .Select(step => step.XPath)
                                     .Distinct()
                                     .ToArray();

            var provider = _profileProviderFactory.Create(userId);
            return await provider.ReadAsync(xPathesToRead);
        }

        private Task<VariantNameDto[]> GetCheckVariantsAsync(TaskDto[] tasks)
        {
            if (tasks.IsNullOrEmpty())
                return Task.FromResult(Array.Empty<VariantNameDto>());

            var variantIds = tasks
                             .SelectMany(x => x.Checks, (_, check) => check.VariantId)
                             .Distinct()
                             .ToArray();

            return _verificationPolicyStorage.GetCheckVariantNamesAsync(variantIds);
        }

        private OutputTaskDto Map(
            TaskDto task,
            IEnumerable<TaskVariantDto> taskVariants,
            IEnumerable<VariantNameDto> checkVariantNames,
            IReadOnlyDictionary<string, string> collectionStepsFormattedData)
        {
            var variant = taskVariants.FirstOrDefault(x => x.VariantId == task.VariantId);
            return _mapper.Map(task, variant, checkVariantNames, collectionStepsFormattedData);
        }
    }
}
