using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Dtos.Profile;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;
using WX.B2C.User.Verification.Extensions;
using TaskExtensions = WX.B2C.User.Verification.Extensions.TaskExtensions;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal interface ICheckFilteringService
    {
        Task<(CheckToInstructContext[], SkippedCheck<AcceptanceCheck>[])> FilterAsync(Guid userId, IEnumerable<AcceptanceCheck> checks);

        Task<(CheckToRunContext[], SkippedCheck<PendingCheck>[])> FilterAsync(Guid userId, IEnumerable<PendingCheck> checks);
    }

    internal class CheckFilteringService : ICheckFilteringService
    {
        private readonly ICheckStorage _checkStorage;
        private readonly ICollectionStepStorage _collectionStepStorage;
        private readonly ITaskStorage _taskStorage;
        private readonly IVerificationPolicyStorage _policyStorage;
        private readonly IProfileProviderFactory _profileProviderFactory;
        private readonly IExternalProfileProvider _externalProfileProvider;
        private readonly IExternalProviderTypeMapper _externalProviderTypeMapper;

        public CheckFilteringService(ICheckStorage checkStorage,
                                     ICollectionStepStorage collectionStepStorage,
                                     ITaskStorage taskStorage,
                                     IVerificationPolicyStorage policyStorage,
                                     IProfileProviderFactory profileProviderFactory, 
                                     IExternalProfileProvider externalProfileProvider, 
                                     IExternalProviderTypeMapper externalProviderTypeMapper)
        {
            _checkStorage = checkStorage ?? throw new ArgumentNullException(nameof(checkStorage));
            _collectionStepStorage = collectionStepStorage ?? throw new ArgumentNullException(nameof(collectionStepStorage));
            _taskStorage = taskStorage ?? throw new ArgumentNullException(nameof(taskStorage));
            _policyStorage = policyStorage ?? throw new ArgumentNullException(nameof(policyStorage));
            _profileProviderFactory = profileProviderFactory ?? throw new ArgumentNullException(nameof(profileProviderFactory));
            _externalProfileProvider = externalProfileProvider ?? throw new ArgumentNullException(nameof(externalProfileProvider));
            _externalProviderTypeMapper = externalProviderTypeMapper ?? throw new ArgumentNullException(nameof(externalProviderTypeMapper));
        }

        public async Task<(CheckToInstructContext[], SkippedCheck<AcceptanceCheck>[])> FilterAsync(Guid userId, IEnumerable<AcceptanceCheck> checks)
        {
            var contexts = checks.Select(CheckToInstructContext.Create).ToArray();

            _ = await TaskExtensions.WhenAll(
                EnrichCheckVariantAsync(contexts),
                EnrichPreviousChecksAsync(contexts, userId));

            var (checksToInstruct, skippedChecks) = Filter(contexts);
            return (checksToInstruct, skippedChecks);
        }

        public async Task<(CheckToRunContext[], SkippedCheck<PendingCheck>[])> FilterAsync(Guid userId, IEnumerable<PendingCheck> checks)
        {
            var contexts = checks.Select(CheckToRunContext.Create).ToArray();
            contexts = await EnrichTaskPriorityGroupsAsync(contexts, userId);
            contexts = await EnrichCollectionStepsAsync(contexts, userId);

            var (checksToRun, skippedChecks) = Filter(contexts);

            _ = await TaskExtensions.WhenAll(
                EnrichInputDataAsync(checksToRun, userId),
                EnrichExternalProfileAsync(checksToRun, userId));

            return (checksToRun, skippedChecks);
        }

        private async Task<CheckToInstructContext[]> EnrichCheckVariantAsync(CheckToInstructContext[] contexts)
        {
            if (contexts.Length == 0) return contexts;

            var uniqueCheckVariantIds = contexts.Select(context => context.Check.VariantId).Distinct().ToArray();
            var checkVariants = await _policyStorage.GetChecksInfoAsync(uniqueCheckVariantIds);

            return contexts
                   .Join(checkVariants,
                       context => context.Check.VariantId,
                       variant => variant.Id,
                       (check, variant) => check.With(variant))
                   .ToArray();
        }

        private async Task<CheckToInstructContext[]> EnrichPreviousChecksAsync(CheckToInstructContext[] contexts, Guid userId)
        {
            if (contexts.Length == 0) return contexts;

            var variantIds = contexts.Select(context => context.Check.VariantId).ToArray();
            var previousChecks = await _checkStorage.GetAsync(userId, variantIds);
            return contexts.Select(EnrichPreviousChecks).ToArray();

            CheckToInstructContext EnrichPreviousChecks(CheckToInstructContext context) =>
                context.With(previousChecks.Where(previousCheck => previousCheck.Variant.Id == context.Check.VariantId));
        }

        private async Task<CheckToRunContext[]> EnrichTaskPriorityGroupsAsync(CheckToRunContext[] contexts, Guid userId)
        {
            if (contexts.Length == 0) return contexts;

            var priorityGroups = await _taskStorage.GetPriorityGroupsAsync(userId);
            return contexts.Select(EnrichPriorityGroups).ToArray();

            CheckToRunContext EnrichPriorityGroups(CheckToRunContext context)
            {
                // TODO: What if check presents in a few tasks and tasks are in different priority groups
                var priority = priorityGroups
                               .FirstOrDefault(group => group.Checks.Contains(context.Check.Id))
                               ?.Priority ?? 0;

                return context.With(priority).With(priorityGroups);
            }
        }

        private async Task<CheckToRunContext[]> EnrichCollectionStepsAsync(CheckToRunContext[] contexts, Guid userId)
        {
            if (contexts.Length == 0) return contexts;

            var uniqueXPathes = contexts.SelectMany(context => context.Check.Parameters)
                                        .Select(parameter => parameter.XPath)
                                        .Distinct()
                                        .ToArray();

            var requestedSteps = await _collectionStepStorage.FindRequestedAsync(userId, uniqueXPathes);
            return contexts.Select(EnrichCollectionSteps).ToArray();

            CheckToRunContext EnrichCollectionSteps(CheckToRunContext context)
            {
                var collectionSteps =
                    context.Check.Parameters
                           .Join(requestedSteps,
                               requiredData => requiredData.XPath,
                               data => data.XPath,
                               (_, data) => data)
                           .ToDictionary(step => step.XPath, step => step);

                return context.With(collectionSteps);
            }
        }

        private async Task<CheckToRunContext[]> EnrichInputDataAsync(CheckToRunContext[] contexts, Guid userId)
        {
            var uniqueXPathes = contexts.SelectMany(context => context.Check.Parameters)
                                        .Select(parameter => parameter.XPath)
                                        .Distinct()
                                        .ToArray();

            var profileDataProvider = _profileProviderFactory.Create(userId);
            var profileData = await profileDataProvider.ReadAsync(uniqueXPathes);

            return contexts.Select(EnrichInputData).ToArray();

            CheckToRunContext EnrichInputData(CheckToRunContext context)
            {
                var inputData = context.Check.Parameters
                                       .Join(
                                           profileData,
                                           parameter => parameter.XPath,
                                           data => data.Key,
                                           (_, data) => data)
                                       .ToDictionary(data => data.Key, data => data.Value);

                return context.With(inputData);
            }
        }

        private async Task<CheckToRunContext[]> EnrichExternalProfileAsync(CheckToRunContext[] contexts, Guid userId)
        {
            var uniqueProviderTypes = contexts.Select(context => context.Check.Provider).Distinct().ToArray();
            var externalProfiles = await uniqueProviderTypes.Select(type => EstablishExternalProfileAsync(userId, type)).WhenAll();
            var externalProfilesMap = externalProfiles.ToDictionary(x => x.Item1, x => x.Item2);

            return contexts.Select(EnrichExternalProfile).ToArray();

            CheckToRunContext EnrichExternalProfile(CheckToRunContext context)
            {
                var externalProfile = externalProfilesMap.GetValueOrDefault(context.Check.Provider);
                return externalProfile != null ? context.With(externalProfile) : context;
            }
        }

        private async Task<(CheckProviderType, ExternalProfileDto)> EstablishExternalProfileAsync(Guid userId, CheckProviderType checkProviderType)
        {
            var externalProviderType = _externalProviderTypeMapper.Map(checkProviderType);

            var externalProfile = externalProviderType.HasValue
                ? await _externalProfileProvider.GetOrCreateAsync(userId, externalProviderType.Value)
                : null;

            return (checkProviderType, externalProfile);
        }

        private static (CheckToInstructContext[], SkippedCheck<AcceptanceCheck>[]) Filter(IEnumerable<CheckToInstructContext> contexts)
        {
            // Filter checks that is pending now.
            // Check should be run on most recent data, so there are no need to instruct new check if check with same variant is pending.
            // Filter checks that exceeds max attempt and should not be automatically instructed anymore.

            if (contexts == null)
                throw new ArgumentNullException(nameof(contexts));

            var checksToInstruct = new List<CheckToInstructContext>();
            var skippedChecks = new List<SkippedCheck<AcceptanceCheck>>();

            foreach (var context in contexts)
            {
                if (HasPendingChecks(context))
                    skippedChecks.Add(SkippedCheck<AcceptanceCheck>.Create(context.Check, "Has pending checks."));
                else
                    checksToInstruct.Add(context);
            }

            return (checksToInstruct.ToArray(), skippedChecks.ToArray());
        }

        private static (CheckToRunContext[], SkippedCheck<PendingCheck>[]) Filter(CheckToRunContext[] contexts)
        {
            // Filter checks which has requested steps.
            // We want to run and fail checks which does not have sufficient data but no collection step requested for this xPath.
            // However, we do not want to start check until collection step with given xPath is requested yet.

            if (contexts == null)
                throw new ArgumentNullException(nameof(contexts));

            var readyToRunChecks = new HashSet<CheckToRunContext>();
            var skippedChecks = new HashSet<SkippedCheck<PendingCheck>>();

            foreach (var context in contexts)
            {
                if (HasRequestedSteps(context))
                    skippedChecks.Add(Skipped(context, "Has requested collection steps."));
                else if (HasIncompletePreviousPriorityGroup(context))
                    skippedChecks.Add(Skipped(context, "Has incomplete previous task priority group."));
                else
                    readyToRunChecks.Add(context);
            }

            // TODO: https://wirexapp.atlassian.net/browse/WRXB-10986
            // Wait all pending Onfido checks due to provider specifics
            var skippedCheckGroups = contexts
                                    .Where(context => context.Check.ShouldBeAggregated)
                                    .GroupBy(context => new { context.Check.Provider, context.Priority })
                                    .Where(IsCheckGroupSkipped);

            foreach (var context in skippedCheckGroups.Flatten())
            {
                readyToRunChecks.Remove(context);
                skippedChecks.Add(Skipped(context, $"Has pending {context.Check.Provider} checks."));
            }

            return (readyToRunChecks.ToArray(), skippedChecks.ToArray());

            bool IsCheckGroupSkipped(IEnumerable<CheckToRunContext> checkGroup) =>
                checkGroup.Any(context => skippedChecks.Any(skipped => context.Check == skipped.Check));

            static SkippedCheck<PendingCheck> Skipped(CheckToRunContext context, string reason) =>
                SkippedCheck<PendingCheck>.Create(context.Check, reason);
        }

        private static bool HasRequestedSteps(CheckToRunContext context) =>
            context.Check.Parameters
                   .Where(data => data.IsRequired)
                   .Any(data => context.RequestedSteps.ContainsKey(data.XPath));

        private static bool HasIncompletePreviousPriorityGroup(CheckToRunContext context)
        {
            var group = context.PriorityGroups.FirstOrDefault(group => group.Priority == context.Priority - 1);

            return group is { IsCompleted: false };
        }

        private static bool HasPendingChecks(CheckToInstructContext context) =>
            context.PreviousChecks.Any(previousCheck =>
                previousCheck.Variant.Id == context.Check.VariantId && previousCheck.State == CheckState.Pending);
    }
}