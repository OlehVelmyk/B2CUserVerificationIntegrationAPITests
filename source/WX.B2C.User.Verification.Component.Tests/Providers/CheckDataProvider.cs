using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Api.Admin.Client;
using WX.B2C.User.Verification.Api.Admin.Client.Models;
using WX.B2C.User.Verification.Component.Tests.Factories;
using WX.B2C.User.Verification.Component.Tests.Fixtures;
using WX.B2C.User.Verification.Component.Tests.Helpers;
using WX.B2C.User.Verification.Extensions;

namespace WX.B2C.User.Verification.Component.Tests.Providers
{
    internal interface ICheckDataProvider
    {
        Task<(Guid, CollectionStepVariantDto)[]> GetRequestedStepsAsync(Guid userId, Guid variantId);
    }

    internal class CheckDataProvider : ICheckDataProvider
    {
        private readonly ICheckProvider _checkProvider;
        private readonly CollectionStepsFixture _stepsFixture;
        private readonly AdminApiClientFactory _adminApiClientFactory;
        private readonly AdministratorFactory _adminFactory;
        private readonly StepVariantComparer _stepVariantComparer;

        public CheckDataProvider(CollectionStepsFixture stepsFixture,
                                 ICheckProvider checkProvider, 
                                 AdminApiClientFactory adminApiClientFactory,
                                 AdministratorFactory adminFactory,
                                 StepVariantComparer stepVariantComparer)
        {
            _stepsFixture = stepsFixture ?? throw new ArgumentNullException(nameof(stepsFixture));
            _checkProvider = checkProvider ?? throw new ArgumentNullException(nameof(checkProvider));
            _adminApiClientFactory = adminApiClientFactory ?? throw new ArgumentNullException(nameof(adminApiClientFactory));
            _adminFactory = adminFactory ?? throw new ArgumentNullException(nameof(adminFactory));
            _stepVariantComparer = stepVariantComparer ?? throw new ArgumentNullException(nameof(stepVariantComparer));
        }

        public async Task<(Guid, CollectionStepVariantDto)[]> GetRequestedStepsAsync(Guid userId, Guid variantId)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var tasks = await adminClient.Tasks.GetAllAsync(userId);
            var variantIds = EvaluatePriorityVariants(variantId, tasks)
                .Concat(EvaluateOnfidoVariants(variantId, tasks))
                .Concat(new[] { variantId })
                .Distinct()
                .ToArray();

            return await GetRequestedStepsAsync(userId, variantIds);
        }

        private async Task<(Guid, CollectionStepVariantDto)[]> GetRequestedStepsAsync(Guid userId, Guid[] variantIds)
        {
            var admin = await _adminFactory.CreateTopSecurityAdminAsync();
            var adminClient = _adminApiClientFactory.Create(admin);

            var steps = await adminClient.CollectionStep.GetAllAsync(userId);
            var requiredData = variantIds.Select(_checkProvider.GetRequiredData).Flatten().Distinct(_stepVariantComparer).ToArray();
            var requiredSteps = steps.Where(step => requiredData.Contains(step.Variant, _stepVariantComparer)).ToArray();

            var absentData = requiredData.Except(requiredSteps.Select(step => step.Variant), _stepVariantComparer);
            var newSteps = await absentData.Foreach(async variant => (await _stepsFixture.RequestAsync(userId, variant, false, true), variant));

            return requiredSteps.Where(step => step.State != CollectionStepState.Completed)
                                .Select(step => (step.Id, step.Variant))
                                .Concat(newSteps)
                                .ToArray();
        }

        private Guid[] EvaluatePriorityVariants(Guid variantId, IList<TaskDto> tasks)
        {
            var task = tasks.FirstOrDefault(task => task.Checks.Any(check => check.Variant.Id == variantId));
            if (task is null)
                return Array.Empty<Guid>();

            var priority = task.Priority - 1;
            return tasks
                .Where(task => task.Priority == priority)
                .SelectMany(task => task.Checks)
                .Where(check => check.State is CheckState.Pending)
                .Select(check => check.Variant.Id)
                .ToArray();
        }

        private Guid[] EvaluateOnfidoVariants(Guid variantId, IList<TaskDto> tasks)
        {
            var checkInfo = _checkProvider.Get(variantId);
            if (checkInfo.Provider is not CheckProviderType.Onfido)
                return Array.Empty<Guid>();

            return tasks
                .SelectMany(task => task.Checks)
                .Where(check => check.Variant.Provider is CheckProviderType.Onfido)
                .Where(check => check.State is CheckState.Pending)
                .Select(check => check.Variant.Id)
                .ToArray();
        }
    }
}
