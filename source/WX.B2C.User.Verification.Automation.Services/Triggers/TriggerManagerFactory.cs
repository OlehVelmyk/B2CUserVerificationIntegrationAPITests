using System;
using WX.B2C.User.Verification.Core.Contracts;
using WX.B2C.User.Verification.Core.Contracts.Storages;
using WX.B2C.User.Verification.Core.Contracts.Triggers;

namespace WX.B2C.User.Verification.Automation.Services.Triggers
{
    internal interface ITriggerManagerFactory
    {
        ITriggerManager Create(Guid userId);
    }

    internal class TriggerManagerFactory : ITriggerManagerFactory
    {
        private readonly ITriggerConditionServiceFactory _conditionServiceFactory;
        private readonly ITriggerService _triggerService;
        private readonly ITriggerStorage _triggerStorage;

        public TriggerManagerFactory(ITriggerConditionServiceFactory conditionServiceFactory, 
                                     ITriggerService triggerService, 
                                     ITriggerStorage triggerStorage)
        {
            _conditionServiceFactory = conditionServiceFactory ?? throw new ArgumentNullException(nameof(conditionServiceFactory));
            _triggerService = triggerService ?? throw new ArgumentNullException(nameof(triggerService));
            _triggerStorage = triggerStorage ?? throw new ArgumentNullException(nameof(triggerStorage));
        }

        public ITriggerManager Create(Guid userId) =>
            new TriggersManager(userId, _conditionServiceFactory.Create(userId), _triggerService, _triggerStorage);
    }
}