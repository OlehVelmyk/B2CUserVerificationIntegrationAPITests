using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Policy;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Worker.Jobs.Jobs.CollectionSteps
{
    internal interface ICollectionStepsPolicyProvider
    {
        Task<CollectionStepsPolicy> GetAsync(Guid policyId);
    }

    /// <summary>
    /// Provides defined in policies collection steps and tasks which depend on such steps
    /// from verification policy and hardcoded monitoring or conditional steps.
    /// Uses cache for optimization.
    /// </summary>
    internal class CollectionStepsPolicyProvider : ICollectionStepsPolicyProvider
    {
        private readonly IVerificationPolicyStorage _policyStorage;
        private readonly ICheckProviderService _checkProviderService;
        private readonly Dictionary<Guid, CollectionStepsPolicy> _cache = new();

        public CollectionStepsPolicyProvider(IVerificationPolicyStorage policyStorage, ICheckProviderService checkProviderService)
        {
            _policyStorage = policyStorage ?? throw new ArgumentNullException(nameof(policyStorage));
            _checkProviderService = checkProviderService ?? throw new ArgumentNullException(nameof(checkProviderService));
        }

        public async Task<CollectionStepsPolicy> GetAsync(Guid policyId)
        {
            if (_cache.TryGetValue(policyId, out var stepsPolicy))
                return stepsPolicy;

            var verificationPolicy = await _policyStorage.GetAsync(policyId);
            var collectionSteps = new List<PolicyCollectionStep>();
            var tasks = new List<TaskVariant>();
            var policyTasks = MergeWithDynamicTasks(verificationPolicy);
            foreach (var policyTask in policyTasks)
            {
                var taskSteps = policyTask.CollectionSteps;
                var checksSteps = await GetChecksCollectionStepsAsync(policyTask.CheckVariants);

                collectionSteps.AddRange(taskSteps);
                collectionSteps.AddRange(checksSteps);

                var xPathes = taskSteps.Select(step => step.XPath)
                                       .Concat(checksSteps.Select(step => step.XPath))
                                       .Distinct()
                                       .ToArray();
                var taskVariant = new TaskVariant
                {
                    Type = policyTask.Type,
                    VariantId = policyTask.VariantId,
                    XPathes = xPathes
                };

                tasks.Add(taskVariant);
            }

            stepsPolicy = new CollectionStepsPolicy
            {
                Steps = MergeSteps(collectionSteps),
                Tasks = tasks.ToArray()
            };
            _cache.Add(policyId, stepsPolicy);
            return stepsPolicy;
        }

        private IEnumerable<TaskVariantDto> MergeWithDynamicTasks(VerificationPolicyDto verificationPolicy)
        {
            var verificationTasks = verificationPolicy.Tasks.ToList();
            var monitoringTasks = HardCodedDynamicTasks.Get(verificationPolicy.Id);
            foreach (var monitoringTask in monitoringTasks)
            {
                var verificationTask = verificationTasks.FirstOrDefault(dto => dto.VariantId == monitoringTask.VariantId);
                if (verificationTask == null)
                {
                    verificationTasks.Add(monitoringTask);
                    continue;
                }

                if (verificationTask.Type != monitoringTask.Type)
                    throw new Exception($"Wrong task type or id in {nameof(HardCodedDynamicTasks)} for {monitoringTask.VariantId}");

                verificationTask.CollectionSteps = verificationTask.CollectionSteps.Concat(monitoringTask.CollectionSteps).ToArray();
            }

            return verificationTasks;
        }

        private PolicyCollectionStep[] MergeSteps(IEnumerable<PolicyCollectionStep> collectionSteps) =>
            collectionSteps.ToLookup(step => step.XPath)
                           .Select(grouping => grouping.Aggregate((mergedStep, newStep) =>
                           {
                               mergedStep.IsRequired |= newStep.IsRequired;
                               mergedStep.IsReviewNeeded |= newStep.IsReviewNeeded;
                               return mergedStep;
                           }))
                           .ToArray();

        private async Task<PolicyCollectionStep[]> GetChecksCollectionStepsAsync(Guid[] checkVariantIds)
        {
            if (checkVariantIds.IsNullOrEmpty())
                return Array.Empty<PolicyCollectionStep>();

            var requiredData = await GetCheckRequiredDataAsync(checkVariantIds);
            return BuildCheckSteps(requiredData).ToArray();
        }

        private async Task<CheckInputParameterDto[]> GetCheckRequiredDataAsync(Guid[] checkVariantIds)
        {
            var checksVariants = await _policyStorage.GetChecksInfoAsync(checkVariantIds);
            var getCheckRequiredData = checksVariants.Select(checkVariant => GetParametersAsync(checkVariant.Id));
            var checkRequiredData = await Task.WhenAll(getCheckRequiredData);

            return checkRequiredData.SelectMany(data => data).ToArray();

            Task<CheckInputParameterDto[]> GetParametersAsync(Guid variantId) => _checkProviderService.GetParametersAsync(variantId);
        }

        private static IEnumerable<PolicyCollectionStep> BuildCheckSteps(IEnumerable<CheckInputParameterDto> checkRequiredData)
        {
            return checkRequiredData
                   .GroupBy(data => data.XPath, SelectRequiredFirst)
                   .Select(data => new PolicyCollectionStep
                   {
                       XPath = data.XPath,
                       IsRequired = data.IsRequired,
                       IsReviewNeeded = false
                   })
                   .ToArray();

            static CheckInputParameterDto SelectRequiredFirst(string xPath, IEnumerable<CheckInputParameterDto> checkData) =>
                checkData.OrderByDescending(data => data.IsRequired).First();
        }
    }
}