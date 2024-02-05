using System;
using System.Threading.Tasks;
using WX.B2C.User.Verification.Core.Contracts.Automation;
using WX.B2C.User.Verification.Core.Contracts.Storages;

namespace WX.B2C.User.Verification.Automation.Services
{
    internal class CollectionStepEventObserver : ICollectionStepEventObserver
    {
        private readonly ICollectionStepStorage _collectionStepStorage;
        private readonly ICollectionStepManager _collectionStepManager;

        public CollectionStepEventObserver(ICollectionStepStorage collectionStepStorage,
                                           ICollectionStepManager collectionStepManager)
        {
            _collectionStepStorage = collectionStepStorage ?? throw new ArgumentNullException(nameof(collectionStepStorage));
            _collectionStepManager = collectionStepManager ?? throw new ArgumentNullException(nameof(collectionStepManager));
        }

        public async Task OnDocumentSubmitted(Guid userId, string[] changes)
        {
            var reason = "Document submitted";
            var changedSteps = await _collectionStepStorage.FindRequestedAsync(userId, changes);
            await _collectionStepManager.TrySubmitAsync(changedSteps, reason);
        }

        public async Task OnSurveySubmitted(Guid userId, string[] changes)
        {
            var reason = "Survey submitted.";
            var changedSteps = await _collectionStepStorage.FindRequestedAsync(userId, changes);
            await _collectionStepManager.TrySubmitAsync(changedSteps, reason);
        }

        public async Task OnPersonalDetailsUpdated(Guid userId, string[] changes)
        {
            var reason = "Complete step as data changed.";
            var changedSteps = await _collectionStepStorage.FindRequestedAsync(userId, changes);
            await _collectionStepManager.TrySubmitAsync(changedSteps, reason);
        }

        public async Task OnVerificationDetailsUpdated(Guid userId, string[] changes)
        {
            var reason = "Complete step as data changed.";
            var changedSteps = await _collectionStepStorage.FindRequestedAsync(userId, changes);
            await _collectionStepManager.TrySubmitAsync(changedSteps, reason);
        }
    }
}
