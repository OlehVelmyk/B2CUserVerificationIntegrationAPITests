using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Core.Contracts.Dtos;
using WX.B2C.User.Verification.Core.Contracts.Providers;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Domain.Models;

namespace WX.B2C.User.Verification.Core.Services
{
    public class ActionService : IActionService
    {
        private readonly IApplicationStorage _applicationStorage;
        private readonly ICollectionStepStorage _collectionStepStorage;
        private readonly IUserActionFactory _userActionFactory;
        private readonly IRegionActionsProvider _regionActionsProvider;

        public ActionService(ICollectionStepStorage collectionStepStorage,
                             IApplicationStorage applicationStorage,
                             IUserActionFactory userActionFactory,
                             IRegionActionsProvider regionActionsProvider)
        {
            _collectionStepStorage = collectionStepStorage ?? throw new ArgumentNullException(nameof(collectionStepStorage));
            _applicationStorage = applicationStorage ?? throw new ArgumentNullException(nameof(applicationStorage));
            _userActionFactory = userActionFactory ?? throw new ArgumentNullException(nameof(userActionFactory));
            _regionActionsProvider = regionActionsProvider ?? throw new ArgumentNullException(nameof(regionActionsProvider));
        }

        public async Task<UserActionDto[]> GetAsync(Guid userId)
        {
            // TODO PHASE 2: ProductType should not be hardcoded.
            // Actions should be filtered in respect to applications?
            // Actions should be requested on one application per time or all at once?
            var productType = ProductType.WirexBasic;
            var state = await _applicationStorage.FindStateAsync(userId, productType);
            if (state is null or ApplicationState.Rejected or ApplicationState.Cancelled)
                return Array.Empty<UserActionDto>();
            
            var isAutomated = await _applicationStorage.IsAutomatedAsync(userId, productType);
            if (!isAutomated)
                return Array.Empty<UserActionDto>();

            var collectionSteps = await _collectionStepStorage.FindRequestedAsync(userId);
            if (collectionSteps.Length == 0)
                return  Array.Empty<UserActionDto>();

            var userActionsOptions = await _regionActionsProvider.GetAsync(userId);

            return userActionsOptions.Actions
                .Join(collectionSteps,
                      action => action.XPath,
                      step => step.XPath,
                      CreateUserAction)
                .ToArray();

            UserActionDto CreateUserAction(ActionOption action, CollectionStepDto step) =>
                _userActionFactory.Create(step.XPath, step.IsRequired, action);
        }
    }
}