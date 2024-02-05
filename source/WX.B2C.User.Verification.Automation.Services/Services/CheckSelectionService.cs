using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal interface ICheckSelectionService
    {
        Task<AcceptanceCheck[]> GetAcceptanceChecksAsync(Guid userId);

        Task<PendingCheck[]> GetPendingChecksAsync(Guid userId);

        Task<PendingCheck> GetPendingCheckAsync(Guid checkId);
    }

    internal class CheckSelectionService : ICheckSelectionService
    {
        private readonly ITaskStorage _taskStorage;
        private readonly ICheckStorage _checkStorage;
        private readonly IVerificationPolicyStorage _policyStorage;
        private readonly ICheckProviderService _checkProviderService;

        public CheckSelectionService(
            ITaskStorage taskStorage,
            ICheckStorage checkStorage,
            IVerificationPolicyStorage policyStorage,
            ICheckProviderService checkProviderService)
        {
            _taskStorage = taskStorage ?? throw new ArgumentNullException(nameof(taskStorage));
            _checkStorage = checkStorage ?? throw new ArgumentNullException(nameof(checkStorage));
            _policyStorage = policyStorage ?? throw new ArgumentNullException(nameof(policyStorage));
            _checkProviderService = checkProviderService ?? throw new ArgumentNullException(nameof(checkProviderService));
        }

        public async Task<AcceptanceCheck[]> GetAcceptanceChecksAsync(Guid userId)
        {
            // To find actual acceptance checks which potentially could be re-run we need:
            // 1) Get all assigned user tasks
            // 2) Get task variants by variant id
            // 3) Get task acceptance checks
            // 4) Group checks by variant id because check variants could be repeated in different tasks

            var acceptanceChecks = await SelectAcceptanceChecksAsync(userId);

            var variantIds = acceptanceChecks.Select(check => check.VariantId);
            var checkParametersMap = await GetCheckInputParametersAsync(variantIds);

            return acceptanceChecks.Select(EnrichCheckParameters).ToArray();

            AcceptanceCheck EnrichCheckParameters(AcceptanceCheck check) => check.With(checkParametersMap[check.VariantId]);
        }

        public async Task<PendingCheck[]> GetPendingChecksAsync(Guid userId)
        {
            var pendingChecks = await _checkStorage.GetPendingAsync(userId);
            return await CreatePendingChecksAsync(pendingChecks);
        }

        public async Task<PendingCheck> GetPendingCheckAsync(Guid checkId)
        {
            var check = await _checkStorage.GetAsync(checkId);
            var pendingChecks = await CreatePendingChecksAsync(new[] { check });
            return pendingChecks.SingleOrDefault();
        }

        private async Task<AcceptanceCheck[]> SelectAcceptanceChecksAsync(Guid userId)
        {
            var tasks = await _taskStorage.GetAllAsync(userId);
            var tasksToVariantsMap = tasks.ToDictionary(task => task.VariantId, task => task);
            var taskVariantIds = tasksToVariantsMap.Keys.ToArray();
            var taskVariants = await _policyStorage.GetTaskVariantsAsync(taskVariantIds);
            return taskVariants
                   .SelectMany(
                       taskVariant => taskVariant.CheckVariants,
                       (taskVariant, checkVariantId) => new
                       {
                           CheckVariantId = checkVariantId,
                           RelatedTask = tasksToVariantsMap[taskVariant.VariantId]
                       })
                   .GroupBy(
                       variant => variant.CheckVariantId,
                       (checkVariantId, grouped) =>
                       {
                           var relatedTasks = grouped.Select(x => x.RelatedTask);
                           return AcceptanceCheck.Create(checkVariantId).With(relatedTasks);
                       })
                   .ToArray();
        }

        private async Task<PendingCheck[]> CreatePendingChecksAsync(IReadOnlyCollection<CheckDto> checks)
        {
            if (checks.Count == 0)
                return Array.Empty<PendingCheck>();

            var pendingChecks = checks.Select(PendingCheck.Create).ToArray();

            var variantIds = pendingChecks.Select(check => check.VariantId);
            var checkParametersMap = await GetCheckInputParametersAsync(variantIds);

            return pendingChecks.Select(EnrichCheckParameters).ToArray();

            PendingCheck EnrichCheckParameters(PendingCheck check) => check.With(checkParametersMap[check.VariantId]);
        }

        private async Task<Dictionary<Guid, CheckInputParameterDto[]>> GetCheckInputParametersAsync(IEnumerable<Guid> variantIds)
        {
            variantIds = variantIds.Distinct();

            var checkParameters = await variantIds.Select(GetParametersAsync).WhenAll();
            return new Dictionary<Guid, CheckInputParameterDto[]>(checkParameters);

            async Task<KeyValuePair<Guid, CheckInputParameterDto[]>> GetParametersAsync(Guid variantId)
            {
                var parameters = await _checkProviderService.GetParametersAsync(variantId);
                return new KeyValuePair<Guid, CheckInputParameterDto[]>(variantId, parameters);
            }
        }
    }
}