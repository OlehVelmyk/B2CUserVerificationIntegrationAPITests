using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Dtos;
using WX.B2C.User.Verification.Facade.Controllers.Admin.Mappers;
using TaskVariantDto = WX.B2C.User.Verification.Core.Contracts.Dtos.Policy.TaskVariantDto;

namespace WX.B2C.User.Verification.Facade.Controllers.Admin.Services
{
    public interface IApplicationAggregationService
    {
        Task<ApplicationDto> AggregateAsync(Core.Contracts.Dtos.ApplicationDto application);

        Task<IEnumerable<ApplicationDto>> AggregateAsync(Core.Contracts.Dtos.ApplicationDto[] applications);
    }

    internal class ApplicationAggregationService : IApplicationAggregationService
    {
        private readonly IVerificationPolicyStorage _policyStorage;
        private readonly IApplicationActionsManager _actionsManager;
        private readonly IApplicationMapper _applicationMapper;

        public ApplicationAggregationService(
            IVerificationPolicyStorage policyStorage,
            IApplicationActionsManager actionsManager,
            IApplicationMapper applicationMapper)
        {
            _policyStorage = policyStorage ?? throw new ArgumentNullException(nameof(policyStorage));
            _actionsManager = actionsManager ?? throw new ArgumentNullException(nameof(actionsManager));
            _applicationMapper = applicationMapper ?? throw new ArgumentNullException(nameof(applicationMapper));
        }

        public async Task<ApplicationDto> AggregateAsync(Core.Contracts.Dtos.ApplicationDto application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            var result = await AggregateAsync(new[] { application });
            return result.Single();
        }

        public async Task<IEnumerable<ApplicationDto>> AggregateAsync(Core.Contracts.Dtos.ApplicationDto[] applications)
        {
            if (applications == null)
                throw new ArgumentNullException(nameof(applications));
            if (!applications.Any()) 
                return Enumerable.Empty<ApplicationDto>();

            var taskVariants = await GetTaskVariantsAsync(applications);
            return applications.Select(application => Map(application, taskVariants));
        }

        private Task<TaskVariantDto[]> GetTaskVariantsAsync(IEnumerable<Core.Contracts.Dtos.ApplicationDto> applications)
        {
            var taskVariantIds = applications
                                 .SelectMany(x => x.Tasks)
                                 .Select(x => x.VariantId)
                                 .Distinct()
                                 .ToArray();

            return _policyStorage.GetTaskVariantsAsync(taskVariantIds);
        }

        private ApplicationDto Map(Core.Contracts.Dtos.ApplicationDto application, IEnumerable<TaskVariantDto> taskVariants)
        {
            var allowedActions = _actionsManager.GetAllowedActions(application);
            return _applicationMapper.Map(application, taskVariants, allowedActions);
        }
    }
}